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

        private void DumpToFile(string filename, object o)
        {
            var jsonString = JsonConvert.SerializeObject(o);
            var filenameFull = Server.MapPath(filename);
            File.WriteAllText(filenameFull, jsonString);
            Debug("ZAZAAZAZAAZAZA" + Server.MapPath(filename));
        }

        private Dictionary<string, T> LoadFromFile<T>(string filename)
        {
            var filenameFull = Server.MapPath(filename);

            Debug(filenameFull);

            if (File.Exists(filenameFull))
            {
                using (StreamReader r = new StreamReader(filenameFull))
                    return JsonConvert.DeserializeObject<Dictionary<string,T>>(r.ReadToEnd());
            } else
            {
                return new Dictionary<string, T>();
            }
        }

        protected void DataAndLocation_Click(object sender, EventArgs e)
        {
            // Read user information and location from client.
            CurrentUser = new User(email.Value, name.Value, photoURL.Value);
            CurrentLocation = new Place(latitude.Value, longitude.Value);
            if (!LocalUsers.ContainsKey(email.Value))
            {
                LocalUsers.Add(CurrentUser.Email, CurrentUser);
                FlushUpdate<User>(UPDATE, CurrentUser);
            }
            UpdateOffers();
        }

        private void FlushUpdate<T>(int i, T t)
        {
            Task.Factory.StartNew(
                   () =>
                   {
                       flyxera.Send(i, t);
                       DumpToFile(USER_FILE, LocalUsers);
                       DumpToFile(OFFER_FILE, LocalOffers);
                   });
            UpdateOffers();
        }

        protected void TestCreateOffer_Click(object sender, EventArgs e)
        {
            // Read offer information from client.
            CurrentOffer = new Offer(
                amount.Value,
                CurrentLocation,
                DateTime.Now,
                shortDesc.Value,
                longDesc.Value,
                CurrentUser);

            LocalOffers.Add(CurrentOffer.Id, CurrentOffer);
            FlushUpdate<Offer>(UPDATE, CurrentOffer);
        }

        protected void AcceptButton_Click(object sender, EventArgs e)
        {
            LocalOffers.Remove(Id.Value);
            Task.Factory.StartNew(
             () =>
             {
                 flyxera.Send(REMOVE, Id.Value);
                 DumpToFile(USER_FILE, LocalUsers);
                 DumpToFile(OFFER_FILE, LocalOffers);
             });
            UpdateOffers();
        }

        protected void ShowAllOffers_Click(object sender, EventArgs e)
        {
            UpdateOffers();
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
            flyxera.RegisterHandler(REMOVE, (Action<string>)delegate (string id) {
                if (LocalOffers.ContainsKey(id))
                    LocalOffers.Remove(id);
            });
            flyxera.RegisterMakeChkpt(delegate (Vsync.View nv)
            {
                foreach (User u in LocalUsers.Values) {
                    flyxera.SendChkpt(u);
                }
                foreach (Offer o in LocalOffers.Values)
                {
                    flyxera.SendChkpt(o);
                }
                flyxera.EndOfChkpt();
            });
            
            flyxera.RegisterLoadChkpt((Action<User>)delegate (User u) {
                LocalUsers[u.Email] = u;
            });
            flyxera.RegisterLoadChkpt((Action<Offer>)delegate (Offer o) {
                LocalOffers[o.Id] = o;
            });
        }


        public void UpdateOffers()
        {
            // Sort offers by distance to p
            var sorted = from o
                         in LocalOffers.Values
                         orderby o.Location.DistanceTo(CurrentLocation) ascending
                         select o;
            
            ListOfOffers.DataSource = sorted.Take<Offer>(200);
            ListOfOffers.DataBind();
        }


        public static void Debug(string message) => System.Diagnostics.Debug.WriteLine("!!FLYXERA: " + message);
    }
}