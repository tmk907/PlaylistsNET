using System;
using System.Collections.Generic;

namespace PlaylistsNET.Models
{
	public class M3uPlaylistEntry : BasePlaylistEntry
	{
        private const string EXTALB = "EXTALB";
        private const string EXTART = "EXTART";

        public M3uPlaylistEntry()
		{
			Properties = new Dictionary<string, string>();
			Comments = new List<string>();
		}

		public TimeSpan Duration { get; set; }
		public string Title { get; set; }

		public string Album 
		{
			get 
			{ 
				return GetPropertyValueOrEmpty(EXTALB); 
			} 
			set 
			{ 
				AddOrUpdateProperty(EXTALB, value); 
			} 
		}

		public string AlbumArtist 
		{ 
			get 
			{
				return GetPropertyValueOrEmpty(EXTART); 
			}
			set
			{ 
				AddOrUpdateProperty(EXTART, value); 
			}
		}

		public Dictionary<string, string> Properties { get; }
		public List<string> Comments { get; set; }

		public void AddOrUpdateProperty(string key, string value)
        {
			if (Properties.ContainsKey(key))
            {
				Properties[key] = value;
            }
            else
            {
				Properties.Add(key, value);
            }
		}

		public string GetPropertyValueOrEmpty(string key)
        {
            if (Properties.ContainsKey(key))
            {
				return Properties[key];
            }
			return "";
		}
	}
}
