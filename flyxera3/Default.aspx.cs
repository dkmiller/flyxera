using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Vsync;

namespace flyxera3
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        const int LOOKUP = 0;
        const int UPDATE = 1;

        private static Group flyxera;
        private static User CurrentUser;
        private static Place CurrentLocation;
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
                    VsyncSystem.Start(true, true);
                }
                if (flyxera == null)
                {
                    flyxera = new Group("flyxera");
                    AddHandlers(flyxera);


                    flyxera.Join();
                }
            }
        }

        protected void DataAndLocation_Click(object sender, EventArgs e)
        {
            firstLoad.Value = "foo";
            flyxera.Send(UPDATE, new User(email.Text, name.Text));

            ListOfOffers.DataSource = LocalUsers.Values.ToList();
            ListOfOffers.DataBind();
        }

        protected void ShowAllOffers_Click(object sender, EventArgs e)
        {
            throw new NotSupportedException("not supported");
        }

        protected void ShowMyOffers_Click(object sender, EventArgs e)
        {
            ListOfOffers.DataSource = LocalOffers.Values.Where(x => x.Offerer == CurrentUser);
            ListOfOffers.DataBind();
        }

        protected void AddHandlers(Group g)
        {
            g.Handlers[UPDATE] += (Action<User>)(u => LocalUsers[u.Email] = u);
            g.Handlers[UPDATE] += (Action<Offer>)(o => LocalOffers[o.Id] = o);
        }

        public List<Offer> BestDeals(User u)
        {
            // Sort offers by distance to p
            var sorted = from o in LocalOffers.Values orderby o.Location.DistanceTo(CurrentLocation) ascending select o;

            // Return the first few offers.
            return sorted.Take<Offer>(15).ToList<Offer>();
        }
    }
}