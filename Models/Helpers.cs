using System;
using System.Configuration;

namespace BYUFagElGamous1_5.Models
{
	public class Helpers
	{
		public static string GetRDSConnectionString()
		{
			var appConfig = ConfigurationManager.AppSettings;



			string dbname = appConfig["RDS_DB_NAME"];



			//if (string.IsNullOrEmpty(dbname)) return null;



			string username = appConfig["RDS_USERNAME"];
			string password = appConfig["RDS_PASSWORD"];
			string hostname = appConfig["RDS_HOSTNAME"];
			string port = appConfig["RDS_PORT"];



			return "Server=" + hostname + ",1433;Database = IntexII" + ";User ID=" + username + ";Password=" + password + "; MultipleActiveResultSets = true";
			//Server = intex - db.cdo8ljcmy95c.us - east - 1.rds.amazonaws.com,1433; Database = IntexII; User Id = admin; Password = group1_5; MultipleActiveResultSets = true
			//return "Data Source=aa15qhx9s42mgmt.chgpbo5ixplt.us-east-1.rds.amazonaws.com;Initial Catalog=ebdb;User ID=dbuser;Password=group211;";
		}
	}
}