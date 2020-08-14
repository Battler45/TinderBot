using System;

namespace TinderBot.Models
{

    public class Rootobject
    {
        public Meta meta { get; set; }
        public Data data { get; set; }
    }

    public class Meta
    {
        public int status { get; set; }
    }

    public class Data
    {
        public UserData[] results { get; set; }
        public int timeout { get; set; }// 1800 000 - 0.5hour
    }

    public class UserData
    {
        public string type { get; set; }
        public User user { get; set; }
        public Facebook facebook { get; set; }
        public Spotify spotify { get; set; }
        public int distance_mi { get; set; }
        public string content_hash { get; set; }
        public int s_number { get; set; }
        public Teaser teaser { get; set; }
        public Teaser[] teasers { get; set; }
        public Instagram instagram { get; set; }
    }

    public class User
    {
        public string _id { get; set; }
        public object[] badges { get; set; }
        public string bio { get; set; }
        public DateTime birth_date { get; set; }
        public string name { get; set; }
        public Photo[] photos { get; set; }
        public int gender { get; set; }
        public Job[] jobs { get; set; }
        public School[] schools { get; set; }
        public City city { get; set; }
        public bool show_gender_on_profile { get; set; }
        public bool is_traveling { get; set; }
        public bool hide_age { get; set; }
        public bool hide_distance { get; set; }
    }

    public class City
    {
        public string name { get; set; }
    }

    public class Photo
    {
        public string id { get; set; }
        public string url { get; set; }
        public Processedfile[] processedFiles { get; set; }
        public string fileName { get; set; }
        public string extension { get; set; }
        public Crop_Info crop_info { get; set; }
    }

    public class Crop_Info
    {
        public User user { get; set; }
        public Algo algo { get; set; }
        public bool processed_by_bullseye { get; set; }
        public bool user_customized { get; set; }
        public class User
        {
            public float width_pct { get; set; }
            public float x_offset_pct { get; set; }
            public float height_pct { get; set; }
            public float y_offset_pct { get; set; }
        }
        public class Algo
        {
            public float width_pct { get; set; }
            public float x_offset_pct { get; set; }
            public float height_pct { get; set; }
            public float y_offset_pct { get; set; }
        }
    }



    public class Processedfile
    {
        public string url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

    public class Job
    {
        public Title title { get; set; }

        public class Title
        {
            public string name { get; set; }
        }
    }

    public class School
    {
        public string name { get; set; }
    }

    public class Facebook
    {
        public object[] common_connections { get; set; }
        public int connection_count { get; set; }
        public object[] common_interests { get; set; }
    }

    public class Spotify
    {
        public bool spotify_connected { get; set; }
    }

    public class Teaser
    {
        public string type { get; set; }
        public string _string { get; set; }
    }

    public class Instagram
    {
        public DateTime last_fetch_time { get; set; }
        public bool completed_initial_fetch { get; set; }
        public Photo1[] photos { get; set; }
        public int media_count { get; set; }
    }

    public class Photo1
    {
        public string image { get; set; }
        public string thumbnail { get; set; }
        public string ts { get; set; }
    }

    class Core
    {
    }
}
