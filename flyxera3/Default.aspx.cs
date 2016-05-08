using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Vsync;

namespace flyxera3
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        const int UPDATE = 0;

        private static Group flyxera;

        private static Place CurrentLocation;
        private static User CurrentUser;
        private static Offer CurrentOffer;

        private static Dictionary<string, User> LocalUsers;
        private static Dictionary<string, Offer> LocalOffers;

        protected void Page_Load(object sender, EventArgs e)
        { 
            if (!IsPostBack)
            {
                Environment.SetEnvironmentVariable("VSYNC_MUTE", "true");
                Environment.SetEnvironmentVariable("VSYNC_LOGGED", "false");

                if (LocalUsers == null)
                    LocalUsers = new Dictionary<string, User> {
                        {"dm635@cornell.edu", new User("dm635@cornell.edu", "Daniel Miller", "URL") },
                        {"awg66@cornell.edu",  new User("awg66@cornell.edu", "Alex Gato", "URL") },
                        {"em569@cornell.edu", new User("em569@cornell.edu", "Ege Mihmanli", "URL") }
                    };
                if (LocalOffers == null)
                    LocalOffers = new Dictionary<string, Offer>();

                if (!VsyncSystem.VsyncIsActive())
                {
                    Msg.RegisterType<User>(0);
                    Msg.RegisterType<Offer>(1);
                    Msg.RegisterType<Place>(2);
                    VsyncSystem.Start(true, true);
                }
                if (flyxera == null)
                {
                    flyxera = new Group("flyxera");
                    AddHandlers();
                    flyxera.Join();
                }
            }

        }

        protected void DataAndLocation_Click(object sender, EventArgs e)
        {
          
            Debug("DataAndLocation_Click");

            // Read user information and location from client.
            CurrentUser = new User(email.Value, name.Value, photoURL.Value);
            CurrentLocation = new Place(latitude.Value, longitude.Value);

            Debug(CurrentUser.Email);

            if (!LocalUsers.ContainsKey(email.Value))
                Task.Factory.StartNew(() => { flyxera.Send(UPDATE, CurrentUser); });
            
            ListOfOffers.DataSource = LocalOffers.Values.ToList();
            ListOfOffers.DataBind();
        }

        protected void TestCreateOffer_Click(object sender, EventArgs e)
        {
//            Debug("TestCreateOffer_Click 1");


            // Read offer information from client.
            CurrentOffer = new Offer(
                amount.Value,
                CurrentLocation,
                DateTime.Now,
                shortDesc.Value,
                longDesc.Value,
                CurrentUser);

            // Send offer information to group. 
            Task.Factory.StartNew(() => {
                flyxera.Send(UPDATE, CurrentOffer);
            });

            ListOfOffers.DataSource = LocalOffers.Values.ToList();
            ListOfOffers.DataBind();
        }

        protected void AcceptButton_Click(object sender, EventArgs e)
        {
            // TODO
        }

        protected void ShowAllOffers_Click(object sender, EventArgs e)
        {
            // TODO: only best offers
            if(LocalOffers != null)
            {
                ListOfOffers.DataSource = LocalOffers.Values.ToList();
                ListOfOffers.DataBind();
            }
        }

        protected void ShowMyOffers_Click(object sender, EventArgs e)
        {
            // TODO: sort by date
            ListOfOffers.DataSource = LocalOffers.Values.Where(x => x.Offerer == CurrentUser);
            ListOfOffers.DataBind();
        }

        protected void AddHandlers()
        {
            flyxera.Handlers[UPDATE] += (Action<User>)delegate (User u)
            {
                if (!LocalUsers.ContainsKey(u.Email))
                    LocalUsers.Add(u.Email, u);
            };
            flyxera.Handlers[UPDATE] += (Action<Offer>)delegate (Offer o) {
                if (!LocalOffers.ContainsKey(o.Id))
                    LocalOffers.Add(o.Id, o);
            };
       
        }

        public List<Offer> BestDeals(User u)
        {
            // Sort offers by distance to p
            var sorted = from o
                         in LocalOffers.Values
                         orderby o.Location.DistanceTo(CurrentLocation) ascending
                         select o;

            // Return the first few offers.
            return sorted.Take<Offer>(200).ToList<Offer>();
        }


        public static void Debug(string message) => System.Diagnostics.Debug.WriteLine("!!FLYXERA: " + message);
    }
}