namespace AuxCrud.ViewModel.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Forms;
    using Helpers;
    using Inputs;
    using Interfaces;
    using Model;
    using Newtonsoft.Json;
    using NHibernate;

    public abstract class ViewModel<TOwner, TViewModel> : IViewModel where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
    {
        protected ViewModel()
        {
        }

        protected ViewModel(TOwner owner)
        {
            Owner = owner;
            Maps = new List<MapComponent<TOwner, TViewModel>>();
            References = new List<ReferenceComponent<TOwner, TViewModel>>();
            Inputs = new List<InputComponent<TOwner, TViewModel>>();

            Map(x => x.Id, y => y.Id);
        }

        public int Id { get; set; }
        [JsonIgnore]
        public TOwner Owner { get; set; }
        [JsonIgnore]
        public IList<MapComponent<TOwner, TViewModel>> Maps { get; set; }
        [JsonIgnore]
        public IList<ReferenceComponent<TOwner, TViewModel>> References { get; set; }
        [JsonIgnore]
        public IList<InputComponent<TOwner, TViewModel>> Inputs { get; set; }

        public FormComponent<TOwner, TViewModel> FormComponent { get; set; }

        public abstract string Readable { get; }

        protected MapComponent<TOwner, TViewModel> Map(Expression<Func<TOwner, object>> ownerExpression, Expression<Func<TViewModel, object>> expression, bool searchable = false)
        {
            var mapComponent = new MapComponent<TOwner, TViewModel>(ownerExpression, expression, searchable, this);

            var property = LambdaHelper.GetMemberExpression(expression).Member as PropertyInfo;
            var ownerProperty = LambdaHelper.GetMemberExpression(ownerExpression).Member as PropertyInfo;
            if (property != null && ownerProperty != null && Owner != null)
            {
                property.SetValue(this, ownerProperty.GetValue(Owner, null), null);
            }
            Maps.Add(mapComponent);
            return mapComponent;
        }

        protected ReferenceComponent<TOwner, TViewModel> Reference(Expression<Func<TOwner, object>> ownerExpression, Expression<Func<TViewModel, object>> expression, bool searchable = false)
        {
            var referenceComponent = new ReferenceComponent<TOwner, TViewModel>(ownerExpression, expression, this);

            var property = LambdaHelper.GetMemberExpression(expression).Member as PropertyInfo;
            var ownerProperty = LambdaHelper.GetMemberExpression(ownerExpression).Member as PropertyInfo;
            if (property != null && ownerProperty != null && Owner != null)
            {
                var viewModelType = property.PropertyType;
                var model = ownerProperty.GetValue(Owner, null);
                var viewModel = Activator.CreateInstance(viewModelType, model);
                property.SetValue(this, viewModel, null);
            }

            References.Add(referenceComponent);
            return referenceComponent;
        }

        public FormComponent<TOwner, TViewModel> Form()
        {
            var formComponent = new FormComponent<TOwner, TViewModel>(this);
            FormComponent = formComponent;
            return formComponent;
        }

        public InputComponent<TOwner, TViewModel> Input(Expression<Func<TViewModel, object>> expression, BaseInput input)
        {
            var columnComponent = new InputComponent<TOwner, TViewModel>(expression, input, this, null);
            Inputs.Add(columnComponent);
            return columnComponent;
        }

        public void BindReferences(ISession session, NameValueCollection parameters)
        {
            foreach (var referenceComponent in this.References)
            {
                var propInfo = LambdaHelper.GetMemberExpression(referenceComponent.Expression).Member as PropertyInfo;
                var ownerPropInfo = LambdaHelper.GetMemberExpression(referenceComponent.OwnerExpression).Member as PropertyInfo;
                if (propInfo != null && ownerPropInfo != null)
                {
                    var paramName = $"item.{propInfo.Name}";
                    var idValue = Convert.ToInt32(parameters[paramName]);
                    if (idValue > 0)
                    {
                        var modelInstance = session.Get(ownerPropInfo.PropertyType, idValue);
                        var viewModelInstance = Activator.CreateInstance(propInfo.PropertyType, modelInstance);
                        propInfo.SetValue(this, viewModelInstance, null);
                    }
                }
            }
        }

        public TOwner Update(ISession session)
        {
            var item = session.Get<TOwner>(Id) ?? new TOwner();

            foreach (var map in Maps)
            {
                var property = LambdaHelper.GetMemberExpression(map.Expression).Member as PropertyInfo;
                var ownerProperty = LambdaHelper.GetMemberExpression(map.OwnerExpression).Member as PropertyInfo;
                if (property != null)
                {
                    ownerProperty?.SetValue(item, property.GetValue(this, null), null);
                }
            }

            foreach (var reference in References)
            {
                var property = LambdaHelper.GetMemberExpression(reference.Expression).Member as PropertyInfo;
                var ownerProperty = LambdaHelper.GetMemberExpression(reference.OwnerExpression).Member as PropertyInfo;
                if (property != null && ownerProperty != null)
                {
                    var modelType = ownerProperty.PropertyType;
                    var viewModel = (IViewModel)property.GetValue(this, null);
                    var model = session.Get(modelType, viewModel.Id);
                    ownerProperty.SetValue(item, model, null);
                }
            }

            return item;
        }

        public Dictionary<string, string> Errors()
        {
            var errors = new Dictionary<string, string>();

            foreach (var inputComponent in Inputs)
            {
                var property = LambdaHelper.GetMemberExpression(inputComponent.Expression).Member as PropertyInfo;
                //var property = ((MemberExpression)inputComponent.Expression.Body).Member as PropertyInfo;
                var value = property?.GetValue(this, null);

                if (!inputComponent.InputField.Isvalid(value) && property != null)
                {
                    errors.Add(property.Name, inputComponent.InputField.GetMessage(property.Name));
                }
            }

            return errors;
        }

        public bool IsValid()
        {
            return Errors().Count == 0;
        }


    }

    public class MapComponent<TOwner, TViewModel> where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
    {
        public MapComponent(Expression<Func<TOwner, object>> ownerExpression, Expression<Func<TViewModel, object>> expression, bool searchable, ViewModel<TOwner, TViewModel> viewModel)
        {
            OwnerExpression = ownerExpression;
            Expression = expression;
            Searchable = searchable;
            ViewModel = viewModel;
        }

        public ViewModel<TOwner, TViewModel> ViewModel { get; set; }
        public Expression<Func<TOwner, object>> OwnerExpression { get; set; }
        public Expression<Func<TViewModel, object>> Expression { get; set; }
        public bool Searchable { get; set; }
    }

    public class ReferenceComponent<TOwner, TViewModel> where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
    {
        public ReferenceComponent(Expression<Func<TOwner, object>> ownerExpression, Expression<Func<TViewModel, object>> expression, ViewModel<TOwner, TViewModel> viewModel)
        {
            OwnerExpression = ownerExpression;
            Expression = expression;
            ViewModel = viewModel;
        }

        public ViewModel<TOwner, TViewModel> ViewModel { get; set; }
        public Expression<Func<TOwner, object>> OwnerExpression { get; set; }
        public Expression<Func<TViewModel, object>> Expression { get; set; }
    }

    public class Input
    {
        public int order;
        public string label;
        public string input;

        public int size;
        public int smallSize;
        public int labelSize;
        public int labelSmallSize;
        public bool showLabel;
    }

}