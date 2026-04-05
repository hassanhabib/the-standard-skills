// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

// TEMPLATE: Standard-compliant Foundation Service
// Replace [Entity] with the entity name (e.g., Student)
// Replace [Namespace] with your project namespace

using System;
using System.Linq;
using System.Threading.Tasks;
using Xeptions;

namespace [Namespace].Services.Foundations.[Entities]
{
    // Root file: [Entity]Service.cs
    public partial class [Entity]Service : I[Entity]Service
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public [Entity]Service(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<[Entity]> Add[Entity]Async([Entity] [entity]) =>
            TryCatch(async () =>
            {
                Validate[Entity]OnAdd([entity]);

                return await this.storageBroker.Insert[Entity]Async([entity]);
            });

        public ValueTask<IQueryable<[Entity]>> RetrieveAll[Entities]() =>
            TryCatch(() =>
            {
                IQueryable<[Entity]> all[Entities] =
                    this.storageBroker.SelectAll[Entities]();

                return ValueTask.FromResult(all[Entities]);
            });

        public ValueTask<[Entity]> Retrieve[Entity]ByIdAsync(Guid [entity]Id) =>
            TryCatch(async () =>
            {
                Validate[Entity]Id([entity]Id);

                [Entity] maybe[Entity] =
                    await this.storageBroker.Select[Entity]ByIdAsync([entity]Id);

                ValidateStorage[Entity](maybe[Entity], [entity]Id);

                return maybe[Entity];
            });

        public ValueTask<[Entity]> Modify[Entity]Async([Entity] [entity]) =>
            TryCatch(async () =>
            {
                Validate[Entity]OnModify([entity]);

                [Entity] maybe[Entity] =
                    await this.storageBroker.Select[Entity]ByIdAsync([entity].Id);

                ValidateStorage[Entity](maybe[Entity], [entity].Id);
                ValidateAgainstStorage[Entity]OnModify(inputEntity: [entity], storage[Entity]: maybe[Entity]);

                return await this.storageBroker.Update[Entity]Async([entity]);
            });

        public ValueTask<[Entity]> Remove[Entity]ByIdAsync(Guid [entity]Id) =>
            TryCatch(async () =>
            {
                Validate[Entity]Id([entity]Id);

                [Entity] maybe[Entity] =
                    await this.storageBroker.Select[Entity]ByIdAsync([entity]Id);

                ValidateStorage[Entity](maybe[Entity], [entity]Id);

                return await this.storageBroker.Delete[Entity]Async(maybe[Entity]);
            });
    }
}

// ---------------------------------------------------------------
// [Entity]Service.Validations.cs
// ---------------------------------------------------------------

namespace [Namespace].Services.Foundations.[Entities]
{
    public partial class [Entity]Service
    {
        private void Validate[Entity]OnAdd([Entity] [entity])
        {
            Validate[Entity]IsNotNull([entity]);

            Validate(
                (Rule: IsInvalid([entity].Id), Parameter: nameof([Entity].Id)),
                (Rule: IsInvalid([entity].Name), Parameter: nameof([Entity].Name)),
                (Rule: IsInvalid([entity].CreatedDate), Parameter: nameof([Entity].CreatedDate)),
                (Rule: IsInvalid([entity].UpdatedDate), Parameter: nameof([Entity].UpdatedDate)),
                (Rule: IsNotSame(
                    firstDate: [entity].CreatedDate,
                    secondDate: [entity].UpdatedDate,
                    secondDateName: nameof([Entity].UpdatedDate)),
                    Parameter: nameof([Entity].UpdatedDate)),
                (Rule: IsNotRecent([entity].CreatedDate), Parameter: nameof([Entity].CreatedDate)));
        }

        private void Validate[Entity]OnModify([Entity] [entity])
        {
            Validate[Entity]IsNotNull([entity]);

            Validate(
                (Rule: IsInvalid([entity].Id), Parameter: nameof([Entity].Id)),
                (Rule: IsInvalid([entity].Name), Parameter: nameof([Entity].Name)),
                (Rule: IsInvalid([entity].CreatedDate), Parameter: nameof([Entity].CreatedDate)),
                (Rule: IsInvalid([entity].UpdatedDate), Parameter: nameof([Entity].UpdatedDate)),
                (Rule: IsSame(
                    firstDate: [entity].UpdatedDate,
                    secondDate: [entity].CreatedDate,
                    secondDateName: nameof([Entity].CreatedDate)),
                    Parameter: nameof([Entity].UpdatedDate)),
                (Rule: IsNotRecent([entity].UpdatedDate), Parameter: nameof([Entity].UpdatedDate)));
        }

        private static void Validate[Entity]IsNotNull([Entity] [entity])
        {
            if ([entity] is null)
            {
                throw new Null[Entity]Exception(message: "[Entity] is null.");
            }
        }

        private static void ValidateStorage[Entity]([Entity] maybe[Entity], Guid [entity]Id)
        {
            if (maybe[Entity] is null)
            {
                throw new NotFound[Entity]Exception([entity]Id);
            }
        }

        private static void Validate[Entity]Id(Guid [entity]Id)
        {
            Validate((Rule: IsInvalid([entity]Id), Parameter: nameof([Entity].Id)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
        {
            Condition = firstDate != secondDate,
            Message = $"Date is not the same as {secondDateName}"
        };

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
        {
            Condition = firstDate == secondDate,
            Message = $"Date is the same as {secondDateName}"
        };

        private dynamic IsNotRecent(DateTimeOffset date)
        {
            var (isNotRecent, startDate, endDate) = IsDateNotRecent(date);

            return new
            {
                Condition = isNotRecent,
                Message = $"Date is not recent. Expected a value between {startDate} and {endDate}"
            };
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalid[Entity]Exception = new Invalid[Entity]Exception(
                message: "Invalid [entity]. Please correct the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalid[Entity]Exception.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalid[Entity]Exception.ThrowIfContainsErrors();
        }
    }
}

// ---------------------------------------------------------------
// [Entity]Service.Exceptions.cs
// ---------------------------------------------------------------

namespace [Namespace].Services.Foundations.[Entities]
{
    public partial class [Entity]Service
    {
        private delegate ValueTask<[Entity]> Returning[Entity]Function();

        private async ValueTask<[Entity]> TryCatch(Returning[Entity]Function returning[Entity]Function)
        {
            try
            {
                return await returning[Entity]Function();
            }
            catch (Null[Entity]Exception null[Entity]Exception)
            {
                throw CreateAndLogValidationException(null[Entity]Exception);
            }
            catch (Invalid[Entity]Exception invalid[Entity]Exception)
            {
                throw CreateAndLogValidationException(invalid[Entity]Exception);
            }
            catch (NotFound[Entity]Exception notFound[Entity]Exception)
            {
                throw CreateAndLogValidationException(notFound[Entity]Exception);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExists[Entity]Exception =
                    new AlreadyExists[Entity]Exception(
                        message: "[Entity] with the same id already exists.",
                        innerException: duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExists[Entity]Exception);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var locked[Entity]Exception =
                    new Locked[Entity]Exception(
                        message: "[Entity] record is locked, please try again.",
                        innerException: dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(locked[Entity]Exception);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failed[Entity]StorageException =
                    new Failed[Entity]StorageException(
                        message: "Failed [entity] storage error occurred, contact support.",
                        innerException: dbUpdateException);

                throw CreateAndLogDependencyException(failed[Entity]StorageException);
            }
            catch (Exception serviceException)
            {
                var failed[Entity]ServiceException =
                    new Failed[Entity]ServiceException(
                        message: "Unexpected service error occurred. Contact support.",
                        innerException: serviceException);

                throw CreateAndLogServiceException(failed[Entity]ServiceException);
            }
        }

        private [Entity]ValidationException CreateAndLogValidationException(Xeption exception)
        {
            var [entity]ValidationException =
                new [Entity]ValidationException(
                    message: "[Entity] validation error occurred, fix the errors and try again.",
                    innerException: exception);

            this.loggingBroker.LogError([entity]ValidationException);

            return [entity]ValidationException;
        }

        private [Entity]DependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var [entity]DependencyValidationException =
                new [Entity]DependencyValidationException(
                    message: "[Entity] dependency validation error occurred, fix the errors.",
                    innerException: exception);

            this.loggingBroker.LogError([entity]DependencyValidationException);

            return [entity]DependencyValidationException;
        }

        private [Entity]DependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var [entity]DependencyException =
                new [Entity]DependencyException(
                    message: "[Entity] dependency error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError([entity]DependencyException);

            return [entity]DependencyException;
        }

        private [Entity]ServiceException CreateAndLogServiceException(Xeption exception)
        {
            var [entity]ServiceException =
                new [Entity]ServiceException(
                    message: "[Entity] service error occurred, contact support.",
                    innerException: exception);

            this.loggingBroker.LogError([entity]ServiceException);

            return [entity]ServiceException;
        }
    }
}
