using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SIG_VETERINARIA.Abstractions.IRepository;
using SIG_VETERINARIA.DTOs.Auth;
using SIG_VETERINARIA.DTOs.Common;
using SIG_VETERINARIA.DTOs.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SIG_VETERINARIA.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration configuration;
        private string _connectionString = "";
        public UserRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            _connectionString = configuration.GetConnectionString("Connection");
        }

        public async Task<ResultDto<int>> Create(UserCreateRequestDto request)
        {
            ResultDto<int> res = new ResultDto<int>();

            try
            {
                using (var cn = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@p_id", request.id);
                    parameters.Add("@p_username", request.username);
                    parameters.Add("@p_password", request.password);
                    parameters.Add("@p_role_id", request.role_id);

                    using (var lector = await cn.ExecuteReaderAsync("SP_CREATE_USER", parameters, commandType: System.Data.CommandType.StoredProcedure))
                    {
                        while (lector.Read())
                        {
                            res.Item = Convert.ToInt32(lector["id"].ToString());
                            res.IsSuccess = Convert.ToInt32(lector["id"].ToString()) > 0 ? true : false;
                            res.Message = Convert.ToInt32(lector["id"].ToString()) > 0 ? "Información guardada correctamente" : "Información no se pudo guardar";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.MessageException = ex.Message;

            }

            return res;
        }

        public async Task<ResultDto<int>> Delete(DeleteDto request)
        {
            ResultDto<int> res = new ResultDto<int>();

            try
            {
                using (var cn = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@p_id", request.id);
                    using (var lector = await cn.ExecuteReaderAsync("SP_DELETE_USER", parameters, commandType: System.Data.CommandType.StoredProcedure))
                    {
                        while (lector.Read())
                        {
                            res.Item = Convert.ToInt32(lector["id"].ToString());
                            res.IsSuccess = Convert.ToInt32(lector["id"].ToString()) > 0 ? true : false;
                            res.Message = Convert.ToInt32(lector["id"].ToString()) > 0 ? "Información eliminada correctamente" : "Información no se pudo eliminar";
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                res.IsSuccess = false;
                res.MessageException = ex.Message;
            }

            return res;
        }

        public async Task<ResultDto<UserListResponseDTO>> GetAll(UserListRequestDto request)
        {
            ResultDto<UserListResponseDTO> res = new ResultDto<UserListResponseDTO>();
            List<UserListResponseDTO> list = new List<UserListResponseDTO>();
            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@p_indice", request.index);
                parameters.Add("@p_limit", request.limit);

                using (var cn = new SqlConnection(_connectionString))
                {
                    list = (List<UserListResponseDTO>)await cn.QueryAsync<UserListResponseDTO>("SP_LIST_USERS", parameters, commandType: System.Data.CommandType.StoredProcedure);
                }
                res.IsSuccess = list.Count > 0 ? true : false;
                res.Message = list.Count > 0 ? "Información encontrada" : "No se encontró información";
                res.Data = list.ToList();
                res.Total = list[0].totalRegisters;
            }
            catch (Exception ex)
            {

                res.IsSuccess = false;
                res.MessageException = ex.Message;
            }
            return res;
        }
        public async Task<UserDetailResponseDto> GetUserByUsername(string username)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@p_username", username);

            using (var cn = new SqlConnection(_connectionString))
            {
                var query = await cn.QueryAsync<UserDetailResponseDto>("SP_GET_USER_BY_USERNAME", parameters, commandType: System.Data.CommandType.StoredProcedure);
                if (query.Any())
                {
                    return query.First();
                }
                throw new Exception("Usuario o contraseña incorrecto");
            }
        }

        public async Task<UserDetailResponseDto> ValidateUser(LoginRequestDto request)
        {
            UserDetailResponseDto user = await GetUserByUsername(request.username);
            if (user.password == request.password)
            {
                return user;
            }
            throw new Exception("Usuario o contraseña incorrecto");
        }

        public async Task<TokenResponseDto> GenerateToken(UserDetailResponseDto request)
        {
            var key = configuration.GetSection("JWTSettings:Key").Value;
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.id.ToString()));
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.username));
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.role_id.ToString()));

            var credentiales = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = credentiales,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(tokenConfig);
            return new TokenResponseDto { Token = token };

        }

        public async Task<AuthResponseDto> Login(LoginRequestDto request)
        {
            UserDetailResponseDto user = await ValidateUser(request);
            var token = await GenerateToken(user);
            return new AuthResponseDto { isSuccess = true, User = user, Token = token.Token };
        }
    }
}
