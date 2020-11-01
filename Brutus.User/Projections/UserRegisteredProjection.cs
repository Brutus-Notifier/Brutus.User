using System;
using Marten.Events.Projections;

namespace Brutus.User.Projections
{
    public class RegisteredUser
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
    
    public class UserRegisteredProjection: ViewProjection<RegisteredUser, Guid>
    {
        public UserRegisteredProjection()
        {
            ProjectEvent<Events.V1.RegistrationUserFinished>(@event => @event.UserId, Persist);
        }

        private void Persist(RegisteredUser view, Events.V1.RegistrationUserFinished @event)
        {
            view.Id = @event.UserId;
            view.Email = @event.Email;
            view.FirstName = @event.FirstName;
            view.LastName = @event.LastName;
            view.IsActive = true;
        }
    }
}