namespace shoppingApp.WebUI.Models
{
    public class AlertMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string AlertType { get; set; }
    }
}

/*
 public string NameToUrl(string name)
        {
            string url = name.ToLower()
                            .Replace("ç", "c")
                            .Replace("ğ", "g")
                            .Replace("ı", "i")
                            .Replace("ö", "o")
                            .Replace("ş", "s")
                            .Replace("ü", "u")
                            .Replace(" ", "-");

            return url;
        }
*/