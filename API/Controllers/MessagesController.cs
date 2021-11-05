using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
  [Authorize]
  public class MessagesController : BaseApiController
  {
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
    {
      _messageRepository = messageRepository;
      _userRepository = userRepository;
      _mapper = mapper;
    }

    [HttpPost]
   public async Task<ActionResult> CreateMessage(CreateMessageDto createMessageDto){
       var username=User.GetUsername();
       if(username == createMessageDto.RecipientUsername.ToLower())
       return BadRequest("You can not send messages to yourself");
       var sender=await _userRepository.GetUserByUserNameAsync(username);
       var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
       if(recipient == null) return NotFound();
       var message= new Message{
           Sender =sender,
           Recipient = recipient,
           SenderName = sender.UserName,
           RecipientName = recipient.UserName,
           Content = createMessageDto.Content
       };
       _messageRepository.AddMessage(message);
       if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));
       return BadRequest("Faild messages");

   }

   [HttpGet]
   public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser([FromQuery]
    MessageParams messageParams){
       messageParams.Username = User.GetUsername();
       var messages=await _messageRepository.GetMessagesForUser(messageParams);
       Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,
        messages.TotalCount, messages.TotalPages);
       return messages;
   }

   [HttpGet("thread/{username}")]
   public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username){
        var currentUsername= User.GetUsername();
        return Ok(await _messageRepository.GetMessageThread(currentUsername,username));
   }

   [HttpDelete("{id}")]
   public async Task<ActionResult> DeleteMessage(int id){

     var username= User.GetUsername();

     var message= await _messageRepository.GetMessage(id);

     if(message.Sender.UserName !=username && message.Recipient.UserName !=username) return Unauthorized();

     if(message.Sender.UserName == username) message.SenderDeleted=true;

     if(message.Recipient.UserName == username) message.RecipientDeleted = true;

     if(message.SenderDeleted && message.RecipientDeleted)

     _messageRepository.DeleteMessage(message);

     if(await _messageRepository.SaveAllAsync()) return Ok();
     
      return BadRequest("problem delete the message.");
   } 
  }
}