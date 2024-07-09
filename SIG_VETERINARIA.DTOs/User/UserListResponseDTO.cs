namespace SIG_VETERINARIA.DTOs.User
{
    public class UserListResponseDTO
    {
        public int id {  get; set; }
        public string username { get; set; }
        public int role_id { get; set; }
        public int totalRegisters { get; set; }
    }
}
