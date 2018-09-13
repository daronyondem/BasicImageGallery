﻿using CoreImageGallery.Data;
using CoreImageGallery.Extensions;
using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreImageGallery.Services
{
    public class FileStorageService : IStorageService
    {
        private const string ImageFolderUri = "userImages";
        private const string ImageFolder = "wwwroot\\" + ImageFolderUri;
        private ApplicationDbContext _dbContext;

        public FileStorageService(ApplicationDbContext dbContext)
        {
            _dbContext = null;
        }
        public async Task AddImageAsync(Stream stream, string originalName, string userName)
        {
            UploadUtilities.GetImageProperties(originalName, out string uploadId, out string fileName);

            string localPath = Path.Combine(ImageFolder, fileName);
            string imageUri = ImageFolderUri + "/" + fileName;

            using (var fileStream = File.Create(localPath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(fileStream);
            }

            await UploadUtilities.RecordImageUploadedAsync(_dbContext, uploadId, fileName, imageUri);
        }

        public async Task<IEnumerable<UploadedImage>> GetImagesAsync()
        {
            var imageList = new List<UploadedImage>();
            var files = Directory.EnumerateFiles(ImageFolder);

            foreach(var file in files)
            {
                var image = new UploadedImage
                {
                    ImagePath = ImageFolderUri + "/" + Path.GetFileName(file)
                };

                imageList.Add(image);
            }

            return imageList;
        }
    }
}
