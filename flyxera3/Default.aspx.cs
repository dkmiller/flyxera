using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Vsync;
using Newtonsoft.Json;

namespace flyxera3
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        const int UPDATE = 0;
        const int REMOVE = 1;
        const string USER_FILE = "flyxera-User.json";
        const string OFFER_FILE = "flyxera-Offer.json";

        private static Group flyxera;

        private static Place CurrentLocation;
        private static User CurrentUser;
        private static Offer CurrentOffer;

        private static Dictionary<string, User> LocalUsers;
        private static Dictionary<string, Offer> LocalOffers;

        // private static int PersistCounter = 0;

        protected void Page_Load(object sender, EventArgs e)
        { 
            if (!IsPostBack)
            {
                Environment.SetEnvironmentVariable("VSYNC_MUTE", "true");
                Environment.SetEnvironmentVariable("VSYNC_LOGGED", "false");

                if (LocalUsers == null)
                    LocalUsers = new Dictionary<string, User>();
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

                    // Only load from file if initializing the group. 
                    if (flyxera.GetSize() == 1)
                    {
                        LocalUsers = LoadFromFile<User>(USER_FILE);
                        LocalOffers = LoadFromFile<Offer>(OFFER_FILE);
                    }

                    /* Debug("before persist");
                     flyxera.Persistent("flyxera");
                     Debug("after persist"); */
                }
            }

        }

        private void DumpToFile(string filename, object o)
        {
            var jsonString = JsonConvert.SerializeObject(o);

            Debug(jsonString);

            File.WriteAllText(filename, jsonString);
        }

        private Dictionary<string, T> LoadFromFile<T>(string filename)
        {
            if (File.Exists(filename))
            {
                using (StreamReader r = new StreamReader(filename))
                    return JsonConvert.DeserializeObject<Dictionary<string,T>>(r.ReadToEnd());
            } else
            {
                return new Dictionary<string, T>();
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
                // Debug("Before make checkpoint: " + LocalOffers.FirstOrDefault().Key);
                // flyxera.MakeCheckpoint(flyxera.GetView());
         /*       foreach (var o in LocalOffers.Values)
                    Debug("Dumping " + o.Id); */
                DumpToFile(USER_FILE, LocalUsers);
                DumpToFile(OFFER_FILE, LocalOffers);
            });

            ListOfOffers.DataSource = LocalOffers.Values.ToList();
            ListOfOffers.DataBind();
        }

        protected void AcceptButton_Click(object sender, EventArgs e)
        {
            Debug("Not implemented");
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
            var sorted = from o
                         in LocalOffers.Values
                         where o.Offerer == CurrentUser
                         orderby DateTime.Parse(o.Time) descending
                         select o;
            
            ListOfOffers.DataSource = sorted;
            ListOfOffers.DataBind();
        }

        protected void AddHandlers()
        {
            flyxera.RegisterHandler(UPDATE, (Action<User>)delegate (User u)
            {
                if (!LocalUsers.ContainsKey(u.Email))
                    LocalUsers.Add(u.Email, u);
            });
            flyxera.RegisterHandler(UPDATE, (Action<Offer>)delegate (Offer o) {
                if (!LocalOffers.ContainsKey(o.Id))
                    LocalOffers.Add(o.Id, o);
            });
            flyxera.RegisterMakeChkpt(delegate (Vsync.View nv)
            {
                Debug("Begin make checkpoint");
                foreach (User u in LocalUsers.Values) {
                    Debug("Sending user " + u.Email);
                    flyxera.SendChkpt(u);
                }
                foreach (Offer o in LocalOffers.Values)
                {
                    Debug("Sending Offer: " + o.ShortDescription);
                    flyxera.SendChkpt(o);
                }
                flyxera.EndOfChkpt();
                Debug("End make checkpoint");
            });
            
            flyxera.RegisterLoadChkpt((Action<User>)delegate (User u) {
                Debug("Load checkpoint(User) " + u.Email);
                LocalUsers[u.Email] = u;
            });
            flyxera.RegisterLoadChkpt((Action<Offer>)delegate (Offer o) {
                Debug("Load checkpoint(Offer)" + o.ShortDescription);
                LocalOffers[o.Id] = o;
            });
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