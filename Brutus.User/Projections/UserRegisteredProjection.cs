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
            ProjectEvent<Domain.Events.V1.UserCreated>(@event => @event.UserId, Persist);
            ProjectEvent<Domain.Events.V1.UserActivated>(@event => @event.UserId, Persist, onlyUpdate: true);
        }

        private void Persist(RegisteredUser view, Domain.Events.V1.UserCreated @event)
        {
            view.Id = @event.UserId;
            view.Email = @event.Email;
            view.FirstName = @event.FirstName;
            view.LastName = @event.LastName;
            view.IsActive = false;
        }
        
        private void Persist(RegisteredUser view, Domain.Events.V1.UserActivated @event)
        {
            view.IsActive = true;
        }
    }
}