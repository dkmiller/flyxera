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
        private static Dictionary<string, User> LocalUsers;
        private static Dictionary<string, Offer> LocalOffers;

        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {
                Environment.SetEnvironmentVariable("VSYNC_MUTE", "true");
                Environment.SetEnvironmentVariable("VSYNC_LOGGED", "false");

                System.Diagnostics.Debug.WriteLine("!!FLYXERA: VsyncIsActive==" + VsyncSystem.VsyncIsActive());

                if (LocalUsers == null)
                    LocalUsers = new Dictionary<string, User>();

                if (!VsyncSystem.VsyncIsActive())
                {
                    Msg.RegisterType<User>(0);
                    Msg.RegisterType<FlyxArray<Table>>(1);
                    VsyncSystem.Start(true, true);
                }
                if (flyxera == null)
                {
                    flyxera = new Group("flyxera");
                    flyxera.DHTEnable(1, 1, 1);

                    AddHandlers(flyxera);


                    flyxera.Join();
                }
            }
        }

        protected void Button_Click(object sender, EventArgs e)
        {
            flyxera.Send(UPDATE, new User(email.Text, name.Text));

            System.Diagnostics.Debug.WriteLine("!!FLYXERA: Sizeof(LocalUsers)==" + LocalUsers.Count);
     
            ListOfUsers.DataSource = LocalUsers.Values.ToList();
            ListOfUsers.DataBind();

        }

        protected void AddHandlers(Group g)
        {
            g.Handlers[UPDATE] += (Action<User>)(u => LocalUsers[u.Email] = u);
            g.Handlers[UPDATE] += (Action<Offer>)(o => LocalOffers[o.Id] = o);
        }
    }
}