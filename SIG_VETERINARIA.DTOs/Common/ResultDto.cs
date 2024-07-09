namespace SIG_VETERINARIA.DTOs.Common
{
    public class ResultDto<T>
    {
        public string Message { get; set; }
        public string MessageException { get; set; }
        public Boolean IsSuccess { get; set; }
        public T Item { get; set; }
        public List<T> Data { get; set; }
        public int Total {  get; set; }
    }
}
