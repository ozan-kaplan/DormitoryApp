using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models.ViewModels
{
    public class JsonResultViewModel<T>
    {
        public JsonResultViewModel()
        {
            NotifyType = JsonResultNotifyType.success;

        }
        public JsonResultNotifyType NotifyType { get; set; }

        public string NotifyTypeName
        {
            get { return NotifyType.ToString(); }

        }

        public string Message { get; set; }
        public T ResponseData { get; set; }

    }

    public enum JsonResultNotifyType
    {
        success = 0, info = 1, warning = 2, error = 3
    }
}