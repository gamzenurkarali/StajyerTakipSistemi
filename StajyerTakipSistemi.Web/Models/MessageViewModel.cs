using StajyerTakipSistemi.Web.Models;

namespace StajyerTakipSistemi.Web.Models
{
    public class MessageViewModel
    {
        public System.Guid LoggedInUserGuid { get; set; }  
        public List<Message> Messages { get; set; }
        public List<SManager> UsersManager { get; set; }
        public List<SIntern> UsersIntern { get; set; }
        //public List<> Users { get; set; }
        public List<NewMessages> NewMessages { get; set; }
        public Guid SelectedUserGuid { get; set; }

        public SIntern TheIntern { get; set; }
        public SManager TheManager { get; set; }
        public List<GroupedMessagesViewModel> Messagesgrouped { get; set; }




    }
    public class GroupedMessagesViewModel
    {
        public DateTime Date { get; set; }
        public List<Message> Messages{ get; set; }
    }


}
