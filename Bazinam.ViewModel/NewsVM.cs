using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bazinam.ViewModel
{
    public class NewsMV
    {
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public int NewsID { get; set; }
        [Required,MaxLength(150),DisplayName("عنوان")]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        [Required]
        public string ReleaseDate { get; set; }
    }
    public class PictureMV
    {

        public int PictureID { get; set; }

        public string PicName { get; set; }
        public string PicUrl { get; set; }


    }

    public class CommentMV
    {

        public int CommentID { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string comment { get; set; }
    }
}