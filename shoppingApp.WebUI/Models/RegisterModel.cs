using System.ComponentModel.DataAnnotations;

namespace shoppingApp.WebUI.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage="Ad alanı boş bırakılamaz.")]    
        public string FirstName { get; set; }

        [Required(ErrorMessage="Soyad alanı boş bırakılamaz.")]    
        public string LastName { get; set; }

        //[Required(ErrorMessage="Ad alanı boş bırakılamaz.")]    
        //public string UserName { get; set; }

        [Required(ErrorMessage="Şifre alanı boş bırakılamaz.")]    
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        /*[Required] 
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string RePassword { get; set; }
        */
    
    
        [Required(ErrorMessage="Email alanı boş bırakılamaz.")]    
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}