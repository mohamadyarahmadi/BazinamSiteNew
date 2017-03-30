using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bazinam.DomainClasses;
using Bazinam.Utilities;
using Bazinam.ViewModel;

namespace Bazinam.ServiceLayer.Contracts
{
   public interface INewsService
   {
        Task<int> TotalOfPicture();
       Task<IList<PictureMV>> GetNewsPicWithPagging(int pageSize, int page = 1);
       Picture GetImage(int id);
        Task<int> TotalOfNews();
        Task<IList<NewsMV>> GetNewsWithPagging(int pageSize, int page = 1);
       Task<int> CreateNews(NewsMV _new,List<string> picList ,string tempPicPath);
       void UploadNewsPic(IEnumerable<FileAsClass> files,string tempPicPath);
       void RemovePic(string fileNames);
       Task<NewsMV> GetNews(int id);
       Task<int> EditNews( NewsMV _new);
       Task<int> Delete(int id);

   }
}
