using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using teacher_rating.Common.Models;
using teacher_rating.Models.ViewModels;
using teacher_rating.Mongodb.Data.Interfaces;

namespace teacher_rating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IFileService _uploadService;


        public UploadController(IFileService uploadService)
        {
            _uploadService = uploadService;
        }

        /// <summary>
        /// Single File Upload
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("PostSingleFile")]
        public async Task<ActionResult> PostSingleFile([FromForm] FileUploadModel fileDetails)
        {
            if (fileDetails == null)
            {
                return BadRequest();
            }

            try
            {
                await _uploadService.PostFileAsync(fileDetails.FileDetails, fileDetails.FileType, fileDetails);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Multiple File Upload
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("PostMultipleFile")]
        public async Task<ActionResult> PostMultipleFile([FromForm] List<FileUploadModel> fileDetails)
        {
            if (fileDetails == null)
            {
                return BadRequest();
            }

            try
            {
                await _uploadService.PostMultiFileAsync(fileDetails);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Download File
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("DownloadFile")]
        public async Task<ActionResult> DownloadFile(List<string> ids)
        {
            try
            {
                await _uploadService.DownloadFileById(ids);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        [HttpGet("get-all/{schoolId}")]
        public async Task<ActionResult> GetAll(string schoolId)
        {
            try
            {
                var result = await _uploadService.GetAll(schoolId);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("download/{id}")]
        public async Task<ActionResult> DownloadFile(string id)
        {
            var file = await _uploadService.GetById(id);
            if (file.Result == ResultRespond.Success)
            {
                byte[] fileBytes = file.Data.FileData;
                string fileName = file.Data.FileName; // Tên tệp tin muốn tải xuống.

                MemoryStream stream = new MemoryStream(fileBytes);
                return File(stream, "application/octet-stream", fileName);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}