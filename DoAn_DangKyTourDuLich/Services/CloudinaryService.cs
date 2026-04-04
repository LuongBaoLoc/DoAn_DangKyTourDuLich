using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace DoAn_DangKyTourDuLich.Services
{
    /// <summary>
    /// Dịch vụ quản lý upload hình ảnh lên Cloudinary
    /// - Giảm tải cho server (không lưu ảnh locally)
    /// - Tối ưu tốc độ tải trang
    /// - CDN tự động phân phối ảnh
    /// - Hỗ trợ transformation/optimization
    /// </summary>
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _config;

        // Giới hạn upload
        private const long MAX_IMAGE_SIZE = 2 * 1024 * 1024; // 2MB
        private const int MAX_IMAGES_PER_REVIEW = 3;
        private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public CloudinaryService(IConfiguration config)
        {
            _config = config;

            // Lấy credentials từ appsettings.json
            var cloudinaryUrl = config["Cloudinary:CloudinaryUrl"] 
                ?? Environment.GetEnvironmentVariable("CLOUDINARY_URL");

            if (string.IsNullOrEmpty(cloudinaryUrl))
                throw new InvalidOperationException("Cloudinary URL không được cấu hình");

            var account = new Account(cloudinaryUrl);
            _cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Upload một ảnh lên Cloudinary
        /// </summary>
        public async Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file)
        {
            try
            {
                // Validate file
                var validation = ValidateUploadFile(file);
                if (!validation.IsValid)
                    return validation;

                // Tạo tên folder cho review images
                var reviewFolder = $"tour-reviews/{DateTime.Now.Year}/{DateTime.Now.Month:D2}";

                // Upload
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = reviewFolder,
                    PublicId = Guid.NewGuid().ToString(),
                    Overwrite = false,
                    EagerAsync = false,
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    return new CloudinaryUploadResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Lỗi upload: {uploadResult.Error.Message}"
                    };
                }

                return new CloudinaryUploadResult
                {
                    IsValid = true,
                    SecureUrl = uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString(),
                    PublicId = uploadResult.PublicId,
                    Size = uploadResult.Bytes
                };
            }
            catch (Exception ex)
            {
                return new CloudinaryUploadResult
                {
                    IsValid = false,
                    ErrorMessage = $"Lỗi upload ảnh: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Upload nhiều ảnh cùng lúc
        /// </summary>
        public async Task<CloudinaryBatchUploadResult> UploadMultipleImagesAsync(IFormFileCollection files)
        {
            var result = new CloudinaryBatchUploadResult();

            // Kiểm tra giới hạn số lượng ảnh
            if (files.Count > MAX_IMAGES_PER_REVIEW)
            {
                result.AddError($"Tối đa {MAX_IMAGES_PER_REVIEW} ảnh cho mỗi review");
                return result;
            }

            // Upload từng ảnh
            var uploadTasks = new List<Task<CloudinaryUploadResult>>();

            foreach (var file in files)
            {
                uploadTasks.Add(UploadImageAsync(file));
            }

            var uploadResults = await Task.WhenAll(uploadTasks);

            foreach (var uploadResult in uploadResults)
            {
                if (uploadResult.IsValid)
                {
                    result.AddSuccess(uploadResult);
                }
                else
                {
                    result.AddError(uploadResult.ErrorMessage ?? "Lỗi upload không xác định");
                }
            }

            return result;
        }

        /// <summary>
        /// Xóa ảnh từ Cloudinary
        /// </summary>
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                return result.Result == "ok";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy URL chuyển đổi (transformation) của ảnh
        /// </summary>
        public string GetOptimizedImageUrl(string publicId, int width = 400, int height = 400)
        {
            try
            {
                // Manually construct URL with transformation
                var cloudName = _cloudinary.Api.Account.Cloud;
                var transformation = $"w_{width},h_{height},c_fill";
                return $"https://res.cloudinary.com/{cloudName}/image/upload/{transformation}/{publicId}";
            }
            catch
            {
                // Fallback: Return basic URL if computation fails
                var cloudName = _cloudinary.Api.Account.Cloud;
                return $"https://res.cloudinary.com/{cloudName}/image/upload/{publicId}";
            }
        }

        /// <summary>
        /// Validate file trước khi upload
        /// </summary>
        private CloudinaryUploadResult ValidateUploadFile(IFormFile file)
        {
            // Kiểm tra file có null
            if (file == null || file.Length == 0)
                return new CloudinaryUploadResult 
                { 
                    IsValid = false, 
                    ErrorMessage = "Vui lòng chọn ảnh" 
                };

            // Kiểm tra kích thước
            if (file.Length > MAX_IMAGE_SIZE)
                return new CloudinaryUploadResult 
                { 
                    IsValid = false, 
                    ErrorMessage = $"Ảnh quá lớn. Tối đa {MAX_IMAGE_SIZE / (1024 * 1024)}MB" 
                };

            // Kiểm tra extension
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(fileExtension))
                return new CloudinaryUploadResult 
                { 
                    IsValid = false, 
                    ErrorMessage = "Định dạng ảnh không được hỗ trợ. Chỉ chấp nhận: JPG, PNG, WebP" 
                };

            // Kiểm tra MIME type
            if (!_allowedMimeTypes.Contains(file.ContentType.ToLower()))
                return new CloudinaryUploadResult 
                { 
                    IsValid = false, 
                    ErrorMessage = "Định dạng MIME type không được hỗ trợ" 
                };

            return new CloudinaryUploadResult { IsValid = true };
        }
    }

    public class CloudinaryUploadResult
    {
        public bool IsValid { get; set; }
        public string? SecureUrl { get; set; }
        public string? PublicId { get; set; }
        public long Size { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class CloudinaryBatchUploadResult
    {
        public List<CloudinaryUploadResult> SuccessfulUploads { get; set; } = new();
        public List<string> Errors { get; set; } = new();

        public bool IsSuccessful => Errors.Count == 0 && SuccessfulUploads.Count > 0;

        public void AddSuccess(CloudinaryUploadResult result)
        {
            SuccessfulUploads.Add(result);
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        public List<string> GetUploadedUrls()
        {
            return SuccessfulUploads
                .Where(u => !string.IsNullOrEmpty(u.SecureUrl))
                .Select(u => u.SecureUrl!)
                .ToList();
        }
    }
}
