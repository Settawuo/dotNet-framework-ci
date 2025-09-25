using System.ComponentModel.DataAnnotations;

namespace WBBEntity.PanelModels
{
    public class LoginPanelModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
