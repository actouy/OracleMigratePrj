using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
    public class JsonParser 
    {
        public Jresult result;
    }

    public class Jresult
    {
        public int status;
        public string code;
        public string message;
        public string subMsg;
        public string except;
        public object Object;
        public List<Data> data;
        public string exception;
    }
    public class Data
    {
        public string householderName;
        public string areaName;
        public string townName;
        public string villageName;
        public string groupName;
        public string identityID;
        public decimal lat;
        public decimal lng;
        public List<ImageData> upImageList;
        public List<ImageData> fileMapingList;
    }
    public class ImageData 
    {
        public string fUrl;
        public string fNickName;
        public string householderId;
        public string identityID;
        public string fName;
    }
}
