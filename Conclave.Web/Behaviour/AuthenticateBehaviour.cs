using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Conclave.Process;
using Conclave.Process.User;

namespace Conclave.Web.Behaviour {

	// the purpose of authentication is to identify the user
	// the user is identified when we can associate them
	// with a user id... that's it

	// somebody is "logged in" by taking a provided user name
	// and password, and associating them with an id
	// this is is then recorded against the user agent
	// in an encrypted cookie

	// user credentials hold some additional role information
	// in the form of a bitmask, but that is the concern of
	// authorisation later, here it's all about the userid

	/// <summary>
	/// A behaviour concerned with authenticating the user.
	/// </summary>
	/// <remarks>
	/// This behaviour should be a singleton, and maintain no instance fields.
	/// Interaction with the authenticated user is via the
	/// <see cref="GenericPrincipal"/> object hanging off
	/// <see cref="WebContext.User"/>
	/// </remarks>

	public class AuthenticateBehaviour: WebBehaviour {

		// this isn't a static method as its going to move to using
		// properties injected from config as instance members
		private HttpCookie _getUserCookie(string userid) {
			// calc the timeout
			const int timeout = 1; // move to config
			// get a ticket
			FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
				1,																		// version
				userid,															// user id
				DateTime.Now,											// creation
				DateTime.Now.AddHours(timeout),	// Expiration
				true,																// Persistent
				""																	// User data
			);
			// now encrypt the ticket
			string encryptedTicket = FormsAuthentication.Encrypt(ticket);
			// create a cookie and add the encrypted ticket to the cookie as data
			HttpCookie authCookie = new HttpCookie("ConclaveCookie", encryptedTicket) {Expires = DateTime.Now.AddYears(1)};
			return authCookie;
		}

		/// <summary>
		/// Creates an instance of the authentication behaviour
		/// that will respond to the message provided.
		/// </summary>
		/// <param name="message">The message to which the behaviour will respond.</param>	
		public AuthenticateBehaviour(string message) : base(message) {}

		public override void Action(Process.IEvent ev, WebContext context) {
			UserCredentials credentials = UserCredentials.Blank;		
			// do we have a uname and upwd with which to "login"?
			if (context.HasParams("_uname", "_upwd")) { // the underscore is to ensure the params aren't copied to output
				// login
				using (IUserCredentialsStore store = context.Services.GetObject<IUserCredentialsStore>("store::user-credentials")) {
					store.Start();
					credentials = store.GetUserCredentials(context.Params["_uname"], context.Params["_upwd"]);
					if (credentials == UserCredentials.Blank) {
						// it may be appropriate to give a message here
						// I prefer not to give any information on login failure by default
						// context.Errors.CreateMessage("Incorrect details.");
					}
				}
			} else {
				// check if already logged in ie. had a login cookie set
				HttpCookie cookie = context.Request.Cookies["ConclaveCookie"];
				if (cookie != null) {
					FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
					if (ticket != null && ticket.Name != UserCredentials.Blank.Id) {
						using (IUserCredentialsStore store = context.Services.GetObject<IUserCredentialsStore>("store::user-credentials")) {
							store.Start();
							credentials = store.GetUserCredentials(ticket.Name); // ticket.Name should be the user id
							// if the id isn't present in the backing store
							// GetUserCredentials will return UserCredentials.Blank
						}		
					}
				}
			}
			// the credentials.Mask is a bitmask representing roles
			// the UserCredentials don't know anything about the classification of role
			// which are external to it, in this case we're treating the mask as 
			// a mask of GlobalRole flags, you could swap in different roles here
			GlobalRole roles = (GlobalRole) credentials.Mask;
			context.User = new GenericPrincipal(credentials, roles.GetRoleNames()); // use context.User.IsInRole("admin") to assert roles
			context.Response.Cookies.Add(_getUserCookie(credentials.Id)); // if there is no record of the users credentials the ID is "anon"
			context.ControlState["user"] = credentials;
		}

	}
}
