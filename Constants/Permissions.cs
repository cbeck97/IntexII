using System.Collections.Generic;

namespace BYUFagElGamous1_5.Constants
{
    public static class Permissions
    {
        //simple function that returns a list of permissions based on the string passed
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Create",
                $"Permissions.{module}.View",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete",
            };
        }
        //
        public static class Burials
        {
            public const string View = "Permissions.Burials.View";
            public const string Create = "Permissions.Burials.Create";
            public const string Edit = "Permissions.Burials.Edit";
            public const string Delete = "Permissions.Burials.Delete";
        }
    }
}
