using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vsync;

namespace flyxera3
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        const int UPDATE = 0;
        const int REMOVE = 1;
        const string USER_FILE = "flyxera-User.json";
        const string OFFER_FILE = "flyxera-Offer.json";
        const string MASTER_URL = "flyxera320160511094352.azurewebsites.net";

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
                Environment.SetEnvironmentVariable("ISIS_UNICAST_ONLY", "true");
                Environment.SetEnvironmentVariable("ISIS_HOSTS", MASTER_URL);

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
                }
            }
        }

        /// <summary>
        /// Writes the object o to the file with name filename in the server's 
        /// App_Data folder as JSON. 
        /// </summary>
        private void WriteToFile(string filename, object o) => 
            File.WriteAllText(
                Server.MapPath(filename),
                JsonConvert.SerializeObject(o));

        /// <summary>
        /// The reverse of DumpToFile. Loads a Dictionary of type string : 
        /// T from the file with name filename in the Server's App_Data. 
        /// </summary>
        private Dictionary<string, T> LoadFromFile<T>(string filename)
        {
            var filenameFull = Server.MapPath(filename);
            
            if (File.Exists(filenameFull))
            {
                using (StreamReader r = new StreamReader(filenameFull))
                    return JsonConvert.DeserializeObject<Dictionary<string,T>>(r.ReadToEnd());
            } else
            {
                return new Dictionary<string, T>();
            }
        }

        /// <summary>
        /// Called when the client POSTs back user and location information. 
        /// </summary>
        protected void DataAndLocation_Click(object sender, EventArgs e)
        {
            // Read user information and location from client.
            CurrentUser = new User(email.Value, name.Value, photoURL.Value);
            CurrentLocation = new Place(latitude.Value, longitude.Value);
            if (!LocalUsers.ContainsKey(email.Value))
            {
                LocalUsers.Add(CurrentUser.Email, CurrentUser);
            }
            FlushUpdate(UPDATE, CurrentUser);
        }

        /// <summary>
        /// Calls flyxera.Send(i,t) in a new thread, writes all changes to 
        /// local files, and updates the client's list of offers. 
        /// </summary>
        private void FlushUpdate<T>(int i, T t)
        {
            Task.Factory.StartNew(
                   () =>
                   {
                       flyxera.Send(i, t);
                       WriteToFile(USER_FILE, LocalUsers);
                       WriteToFile(OFFER_FILE, LocalOffers);
                   });
            UpdateOffers();
        }

        /// <summary>
        /// Called when the client creates a new offer. Creates the 
        /// corresponding server-side object, adds it to the local dictionary, 
        /// and sends the new data. 
        /// </summary>
        protected void TestCreateOffer_Click(object sender, EventArgs e)
        {
            InitializeIfNull();

            // Read offer information from client.
            CurrentOffer = new Offer(
                amount.Value,
                CurrentLocation,
                DateTime.Now,
                shortDesc.Value,
                longDesc.Value,
                CurrentUser);

            LocalOffers.Add(CurrentOffer.Id, CurrentOffer);
            FlushUpdate(UPDATE, CurrentOffer);
        }

        private void InitializeIfNull()
        {
            if (CurrentLocation == null)
                CurrentLocation = new Place("42.444956", "-76.480994");
            if (CurrentUser == null)
                CurrentUser = new User("hello@example.com", "Jane Doe", "URL");
        }

        protected void AcceptButton_Click(object sender, EventArgs e)
        {
            LocalOffers.Remove(Id.Value);
            FlushUpdate(REMOVE, Id.Value);
        }

        protected void ShowAllOffers_Click(object sender, EventArgs e) => UpdateOffers();

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

        /// <summary>
        /// Registers UPDATE, REMOVE, and checkpoint handlers with flyxera. 
        /// </summary>
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
            flyxera.RegisterHandler(REMOVE, (Action<string>)delegate (string id) {
                if (LocalOffers.ContainsKey(id))
                    LocalOffers.Remove(id);
            });
            flyxera.RegisterMakeChkpt(delegate (View v)
            {
                flyxera.SendChkpt(LocalUsers.Values.ToArray());
                flyxera.SendChkpt(LocalOffers.Values.ToArray());
                flyxera.EndOfChkpt();
            });
            
            flyxera.RegisterLoadChkpt((Action<User[]>)delegate (User[] us) {
                foreach(var u in us)
                    LocalUsers[u.Email] = u;
            });
            flyxera.RegisterLoadChkpt((Action<Offer[]>)delegate (Offer[] os) {
                foreach(var o in os)
                    LocalOffers[o.Id] = o;
            });
        }


        /// <summary>
        /// Computes the closest offers to CurrentUser and updates the client's 
        /// repeater accordingly. 
        /// </summary>
        public void UpdateOffers()
        {
            InitializeIfNull();

            // Sort offers by distance to p
            var sorted = from o
                         in LocalOffers.Values
                         orderby o.Location.DistanceTo(CurrentLocation) ascending
                         select o;
            
            ListOfOffers.DataSource = sorted.Take(20);
            ListOfOffers.DataBind();
        }
    }
}