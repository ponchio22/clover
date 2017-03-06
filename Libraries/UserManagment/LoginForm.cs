using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace Valutech.UserManagment
{
    public delegate void LoginCompletedHandler();

    public partial class LoginForm : Form
    {

        public ValutechUserLogClient client = null;

        private delegate void Invoker();

        public delegate void SetEnabledStatesInvoker(bool logged);

        public event LoginCompletedHandler LoginCompleted;

        private string user;
        private string pass;

        /// <summary>
        /// Constructor
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();
            this.ActiveControl = this.userTextBox;
            this.AcceptButton = this.loginButton;

            client = ValutechUserLogClient.GetInstance();
            client.LoggedOut += new ValutechUserLogClient.LoggedOutEventHandler(client_LoggedOut);

            SetEnabledStates(client.Logged);
        }

        #region Cross Thread Operations

        void client_LoggedOut()
        {
            if (this.InvokeRequired)
                this.Invoke(new SetEnabledStatesInvoker(SetEnabledStates), false);
            else
                SetEnabledStates(false);
        }

        #endregion

        /// <summary>
        /// Set the enabled states based on the log status given
        /// </summary>
        /// <param name="logged"></param>
        private void SetEnabledStates(bool logged)
        {
            try
            {
                userTextBox.Text = String.Empty;
                passwordTextBox.Text = String.Empty;
                userTextBox.ReadOnly = logged;
                passwordTextBox.ReadOnly = logged;
                loginButton.Enabled = !logged;
                if (!logged) userTextBox.Focus();
                loginButton.Text = (logged && !this.client.Logged)? "Accesando...":"Accesar";
            }
            catch
            {
            }
        }

        /// <summary>
        /// Form load handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoginForm_Load(object sender, System.EventArgs e)
        {
            this.Visible = true;
            userTextBox.Focus();
        }

        /// <summary>
        /// Login button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginButton_Click(object sender, EventArgs e)
        {
            if (userTextBox.Text != String.Empty && passwordTextBox.Text != String.Empty)
            {
                user = userTextBox.Text;
                pass = passwordTextBox.Text;
                SetEnabledStates(true);
                Thread loginThread = new Thread(new ThreadStart(Login));
                loginThread.Start();
            }
            else
            {
                MessageBox.Show(this, "No ingresaste tu usuario y/o contraseña");                
            }
        }

        private void Login()
        {
            if (!client.Login(user, pass))
            {
                if (this.InvokeRequired)
                    this.Invoke(new Invoker(LoginFailed));
                else
                    LoginFailed();
            }
            else
            {
                if (LoginCompleted != null) LoginCompleted();
            }
        }

        private void LoginFailed()
        {
            MessageBox.Show(this, "El usuario y/o contraseña son incorrectos");
            SetEnabledStates(false);
        }

        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
