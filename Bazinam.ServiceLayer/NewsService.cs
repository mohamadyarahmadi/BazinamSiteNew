using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;
using Bazinam.DataAccessLayer;
using Bazinam.DomainClasses;
using Bazinam.ViewModel;
using System.Linq;
using Bazinam.ServiceLayer.Contracts;
using Bazinam.Utilities;

namespace Bazinam.ServiceLayer
{
   public class NewsService :INewsService
    {
        private readonly IDbSet<News> _news;
        private readonly IDbSet<Picture> _picture;
        private readonly IDbSet<Comment> _comments;
        private readonly IUnitOfWork _unitOfWork;

       public NewsService(IUnitOfWork unitOfWork)
       {
           _unitOfWork = unitOfWork;
           _news = unitOfWork.Set<News>();
           _picture = unitOfWork.Set<Picture>();
           _comments = unitOfWork.Set<Comment>();
       }

       public async Task<int> TotalOfPicture()
       {
            return await _picture.CountAsync();
        }

       public async Task<IList<PictureMV>> GetNewsPicWithPagging(int pageSize=10, int page = 1)
       {
            var PictureVM = await _picture
                  .Select(x => new PictureMV()
                  {
                      PictureID = x.PictureID,
                      PicName = x.PicName,
                      PicUrl = x.PicUrl
                  }).OrderBy(row => row.PictureID).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
           return PictureVM;
       }

        public async Task<Picture> GetImage(int id)
        {
           return await _unitOfWork.FindAsyncc<Picture>(id);
        }

        public async Task<int> TotalOfNews()
        {
            return  await _unitOfWork.Set<News>().CountAsync();
        }

        public async Task<IList<NewsMV>> GetNewsWithPagging(int pageSize, int page = 1)
        {
            var Newses = await _news
                  .Select(x => new NewsMV()
                  {
                      NewsID = x.NewsID,
                      Title = x.Title,
                      Content = x.Content.Substring(0, x.Content.Length > 50 ? 50 : x.Content.Length),
                      ReleaseDate = x.ReleaseDate.ToString()
                  }).OrderBy(row => row.NewsID).Skip((page - 1) * 10).Take(pageSize).ToListAsync();
            return Newses;
        }

        public async Task<NewsMV> GetNews(int id)
        {
            var result = await _news.Where(i => i.NewsID == id)
                    .Select(x => new Bazinam.ViewModel.NewsMV()
                    {
                        NewsID = x.NewsID,
                        Title = x.Title,
                        Content = x.Content,
                        ReleaseDate = x.ReleaseDate.ToString()
                    }).FirstAsync();
            return result;
        }

        public async Task<int> CreateNews(NewsMV _new,List<string> picList,string tempPicPath )
        {
            News news = new News()
            {
                Title = _new.Title,
                Content = _new.Content,
                ReleaseDate = DateTime.Parse(_new.ReleaseDate)
            };
            foreach (var fileName in picList)
            {
                byte[] result;
                using (FileStream SourceStream = System.IO.File.Open(tempPicPath + fileName, FileMode.Open))
                {
                    result = new byte[SourceStream.Length];
                    await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
                }
                Picture pic = new Picture()
                {
                    PicName = fileName,
                    PicSourceBytes = result,
                    PicUrl = "",
                    IsRefrence = false
                };
                news.PicturescCollection.Add(pic);
            }
            _news.Add(news);
            return await _unitOfWork.SaveAllChangesAsync(true);
        }

        public async void UploadNewsPic(IEnumerable<FileAsClass> files, string tempPicPath)
        {
            foreach (var file in files)
            {
                using (System.IO.FileStream _FileStream =
                            new System.IO.FileStream(tempPicPath+file.FileName, System.IO.FileMode.Create,
                                  System.IO.FileAccess.Write))
                {
                    await _FileStream.WriteAsync(file.FileBuffer,0, file.FileBuffer.Length);
                    _FileStream.Close();
                }
            }
        }

        public void RemovePic(string fileNames)
        {
            if (System.IO.File.Exists(fileNames))
            {
                // The files are not actually removed in this demo
                System.IO.File.Delete(fileNames);
            }
        }
        
        public async Task<int> EditNews( NewsMV _new)
        {
            var editedNews = new News()
            {
                NewsID = _new.NewsID,
                Title = _new.Title,
                Content = _new.Content,
                ReleaseDate = DateTime.Parse(_new.ReleaseDate)
            };
            _unitOfWork.Entry(editedNews).State = System.Data.Entity.EntityState.Modified;
            return await _unitOfWork.SaveAllChangesAsync();
        }

        public async Task<int> Delete(int id)
        {
            var deletedNews = await _unitOfWork.FindAsyncc<News>(id);
            _news.Remove(deletedNews);
            return await _unitOfWork.SaveAllChangesAsync();
        }

        public async Task<IList<CommentMV>> GetComments(int pageSize, int page)
        {
            var Comments = await _comments
                 .Select(x => new CommentMV()
                 {
                     CommentID = x.CommentID,
                     Name = x.Name,
                     comment = x.comment.Substring(0, x.comment.Length > 50 ? 50 : x.comment.Length),
                     ReleaseDate = x.ReleaseDate.ToString()
                 }).OrderBy(row => row.CommentID).Skip((page - 1) * 10).Take(pageSize).ToListAsync();
            return Comments;
        }

        public async Task<CommentMV> GetComment(int id)
        {
            var result = await _comments.Where(i => i.CommentID == id)
                    .Select(x => new Bazinam.ViewModel.CommentMV()
                    {
                        CommentID = x.CommentID,
                        Name = x.Name,
                        comment = x.comment.Substring(0, x.comment.Length > 50 ? 50 : x.comment.Length),
                        state = x.IsAllowed,
                        ReleaseDate = x.ReleaseDate.ToString()
                    }).FirstAsync();
            return result;
        }

        public async Task<int> ChangeCommentState(int id, bool state)
        {
            var editedComment =await _unitOfWork.FindAsyncc<Comment>(id);
            editedComment.IsAllowed = state;
            _unitOfWork.Entry(editedComment).State = System.Data.Entity.EntityState.Modified;
            return await _unitOfWork.SaveAllChangesAsync();
        }

        public async Task<int> TotalOfComment()
        {
            return await _comments.CountAsync();
        }
    }
}
