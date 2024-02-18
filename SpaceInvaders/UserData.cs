using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    public class UserData
    {
        /// <summary>
        /// User name 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User score
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Returns a list of user names 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<UserData> ReadUserData(string filePath)
        {
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (List<UserData>)serializer.Deserialize(file, typeof(List<UserData>)) ?? new List<UserData>();
            }

        }

        /// <summary>
        /// Adds a User to the file named filePath
        /// </summary>
        /// <param name="filePath"></param>
        public void AddUserData(string filePath)
        {
            List<UserData> users;
            bool MAJ = false;

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read existing data
                users = ReadUserData(filePath);
            }
            else
            {
                // Create a new list if the file does not exist
                users = new List<UserData>();
            }

            foreach (var Use in users)
            {
                if(Use.UserName == this.UserName && this.Score > Use.Score)
                {
                    Use.Score = Score;
                    MAJ = true;
                }
            }
            // Add new user
            if (!MAJ) users.Add(this);

            // Rewrite or create the file with the updated data
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, users);
            }
        }

        /// <summary>
        /// returns a list of users if any
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static public List<UserData> Get_users(string filePath)
        {
            if (File.Exists(filePath))
            {
                // Lire les données existantes
                using (StreamReader file = File.OpenText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return (List<UserData>)serializer.Deserialize(file, typeof(List<UserData>)) ?? new List<UserData>();
                }
            }
            List<UserData> users = new List<UserData>();
            return users;
        }

        /// <summary>
        /// the ToString() which will be needed to display the Name and Score 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return UserName + " (" + Score + ")";
        }


    }

}
