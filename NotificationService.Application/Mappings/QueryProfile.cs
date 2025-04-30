using NotificationService.Domain.Models.Entities;
using NotificationService.Shared.Configurations;
using System.Linq.Expressions;

namespace NotificationService.Application.Mappings
{
    /// <summary>
    /// Defines and registers custom query mapping rules for specific entities. The Configure method is a placeholder 
    /// for custom query logic that can be used to override default behavior when filtering by certain properties.
    /// Not needed for basic queries where filter property names match entity property names exactly.
    /// </summary>
    public class QueryProfile
    {
        public void Configure()
        {
            //TODO: Find alternatives to building the query with contains conditions as it can be heavy on the db as the history of sent notifications grows
            // Register configurations for EmailNotification
            QueryProfileConfig.Register<EmailNotification>(config =>
            {
                config.ForProperty("ToRecipients", (entityProperty, value) =>
                {
                    return BuildEmailContainsCondition(entityProperty, value);
                });
                config.ForProperty("CcRecipients", (entityProperty, value) =>
                {
                    return BuildEmailContainsCondition(entityProperty, value);
                });

                config.ForProperty("BccRecipients", (entityProperty, value) =>
                {
                    return BuildEmailContainsCondition(entityProperty, value);
                });
            });
        }

        private static Expression BuildEmailContainsCondition(Expression entityProperty, object value)
        {
            // Ensure 'value' is not null and split only if it contains a valid string
            if (value is not string emailList || string.IsNullOrWhiteSpace(emailList))
            {
                return Expression.Constant(false);
            }

            // Split email list into individual emails
            var emails = emailList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Expression emailContainsCondition = Expression.Constant(false);

            // Build the condition for each email in the list
            foreach (var email in emails)
            {
                var emailConstant = Expression.Constant(email.Trim());
                var containsExpression = Expression.Call(entityProperty, "Contains", null, emailConstant);
                emailContainsCondition = Expression.OrElse(emailContainsCondition, containsExpression);
            }

            return emailContainsCondition;
        }

        /// <summary>
        /// Entry point to initialize query mappings at application startup.
        /// Should be called once during app startup (e.g., in Program.cs).
        /// </summary>
        public static void InitializeMappings()
        {
            var profile = new QueryProfile();
            profile.Configure();
        }
    }
}
