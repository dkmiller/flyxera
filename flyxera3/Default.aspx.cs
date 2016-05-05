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

        static private Group flyxera;
        private static List<User> LocalUsers;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Environment.SetEnvironmentVariable("VSYNC_MUTE", "true");
                Environment.SetEnvironmentVariable("VSYNC_LOGGED", "false");

                System.Diagnostics.Debug.WriteLine("!!FLYXERA: VsyncIsActive==" + VsyncSystem.VsyncIsActive());

                if (!VsyncSystem.VsyncIsActive())
                {
                    Msg.RegisterType<User>(0);
         //           Msg.RegisterType<FlyxArray<Table>>(1);
                    VsyncSystem.Start(true, true);
//                    Msg.RegisterType(typeof(List<User>), 2);
                }
                if (flyxera == null)
                {
                    flyxera = new Group("flyxera");
                    flyxera.DHTEnable(1, 1, 1);

                    flyxera.Join();
                }
            }
        }

        protected void Button_Click(object sender, EventArgs e)
        {
            flyxera.DHTOrderedPut<string, User>(email.Text, new User(email.Text, name.Text));


            ListOfData.DataSource = LocalDht();
            ListOfData.DataBind();

        }

        private IEnumerable<User> LocalDht()
        {
            return flyxera.DHT<string, User>().ToList().Select(k => k.Value);
        }
    }
}