using Gymify.Application.DTOs.Comment;
using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Enums;
using Gymify.Web.ViewModels; // Де лежить ViewModel
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService; // Твій сервіс

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }


        [HttpPost]
        public async Task<IActionResult> AddComment(Guid targetId, CommentTargetType targetType, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return BadRequest("Content required");

            try
            {
                var userId = Guid.Parse(User.FindFirst("UserProfileId")?.Value ?? Guid.Empty.ToString());
                
                var commentDto = await _commentService.UploadComment(userId, targetId, targetType, content);

                return Ok(commentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            await _commentService.DeleteCommentByIdAsync(commentId);
            return Ok();
        }
    }
}