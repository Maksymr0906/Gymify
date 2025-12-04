using Gymify.Application.DTOs.Comment;
using Gymify.Application.Services.Implementation;
using Gymify.Application.Services.Interfaces;
using Gymify.Data.Enums;
using Gymify.Application.ViewModels; // Де лежить ViewModel
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymify.Web.Controllers
{
    [Authorize]
    public class CommentController : BaseController
    {
        private readonly ICommentService _commentService; // Твій сервіс

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }


        [HttpPost]
        public async Task<IActionResult> AddComment([FromForm] CreateCommentRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                // Формуємо словник помилок, як у попередньому прикладі
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                                       .ToDictionary(
                                            kvp => kvp.Key,
                                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                                       );

                return BadRequest(new { success = false, message = "Validation Error", errors = errors });
            }

            try
            {
                var userId = Guid.Parse(User.FindFirst("UserProfileId")?.Value ?? Guid.Empty.ToString());
                var commentDto = await _commentService.UploadComment(userId, request.TargetId, request.TargetType, request.Content);

                return Ok(commentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditComment([FromForm] EditCommentRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                                       .ToDictionary(
                                            kvp => kvp.Key,
                                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                                       );

                return BadRequest(new { success = false, message = "Validation Error", errors = errors });
            }

            try
            {
                var userId = Guid.Parse(User.FindFirst("UserProfileId")?.Value ?? Guid.Empty.ToString());
                await _commentService.UpdateCommentAsync(request.CommentId, userId, request.NewContent);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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