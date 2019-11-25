using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{
    public class ApiResponse<T> where T : class
    {
        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public T ResultData { get; set; }

    }
}
