using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GeckosoftBackend.Models {
    public class ImageModel {
        //public int Width { get; set; }
        //public int Height { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public Uri BaseUrl { get; set; }
        public string Url => new Uri(BaseUrl, Name).ToString();

        public ImageModel() {
        }

        public ImageModel(string name, Uri baseUrl) {
            Name = name;
            BaseUrl = baseUrl;
        }

        protected bool Equals(ImageModel other) {
            return Name == other.Name && Equals(BaseUrl, other.BaseUrl);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ImageModel) obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name, BaseUrl);
        }

        public static bool operator ==(ImageModel left, ImageModel right) {
            return Equals(left, right);
        }

        public static bool operator !=(ImageModel left, ImageModel right) {
            return !Equals(left, right);
        }
    }
}