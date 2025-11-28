namespace Gymify.Data.Enums;

public enum UserRelationshipStatus
{
    None,           // Ніхто (можна додати)
    Friend,         // Вже друзі
    RequestSent,    // Я відправив йому заявку (можу скасувати)
    RequestReceived // Він відправив мені заявку (можу прийняти)
}