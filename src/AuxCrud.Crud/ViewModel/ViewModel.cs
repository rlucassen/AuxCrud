namespace AuxCrud.ViewModel.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Helpers;
    using Inputs;
    using Model;
    using Newtonsoft.Json;
    using NHibernate;

    public abstract class ViewModel<TOwner, TViewModel> where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
    {
        protected ViewModel()
        {
        }

        protected ViewModel(TOwner owner)
        {
            Owner = owner;
            Maps = new List<MapComponent<TOwner, TViewModel>>();
            Inputs = new List<InputComponent<TOwner, TViewModel>>();

            Map(x => x.Id, y => y.Id);
        }

        public int Id { get; set; }
        [JsonIgnore]
        public TOwner Owner { get; set; }
        [JsonIgnore]
        public IList<MapComponent<TOwner, TViewModel>> Maps { get; set; }
        [JsonIgnore]
        public IList<InputComponent<TOwner, TViewModel>> Inputs { get; set; }

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

        public InputComponent<TOwner, TViewModel> Input(Expression<Func<TViewModel, object>> expression, BaseInput input)
        {
            var columnComponent = new InputComponent<TOwner, TViewModel>(expression, input, this);
            Inputs.Add(columnComponent);
            return columnComponent;
        }

        public List<Input> GetInputs(ISession session)
        {
            var inputs = new List<Input>();

            var index = 1;
            foreach (var inputComponent in Inputs)
            {
                var propInfo = LambdaHelper.GetMemberExpression(inputComponent.Expression).Member as PropertyInfo;
                var value = propInfo.GetValue(this, null);
                var inputHtml = inputComponent.Input.Render(propInfo.Name, value, propInfo.Name, session);
                var input = new Input
                {
                    order = index,
                    label = propInfo.Name,
                    input = inputHtml,
                    size = inputComponent.Input.Size,
                    smallSize = inputComponent.Input.SmallSize,
                    labelSize = inputComponent.Input.LabelSize,
                    labelSmallSize = inputComponent.Input.LabelSmallSize,
                    showLabel = inputComponent.Input.ShowLabel
                };

                inputs.Add(input);
                index++;
            }
            return inputs.OrderBy(x => x.order).ToList();
        }


        public TOwner Update(ISession session)
        {
            var item = session.Get<TOwner>(Id) ?? new TOwner();

            foreach (var map in Maps)
            {
                var property = LambdaHelper.GetMemberExpression(map.Expression).Member as PropertyInfo;
                var ownerProperty = LambdaHelper.GetMemberExpression(map.OwnerExpression).Member as PropertyInfo;
                if (property != null && ownerProperty != null)
                {
                    ownerProperty.SetValue(item, property.GetValue(this, null), null);
                }
            }

            return item;
        }

        public Dictionary<string, string> Errors()
        {
            var errors = new Dictionary<string, string>();

            foreach (var inputComponent in Inputs)
            {
                var property = ((MemberExpression)inputComponent.Expression.Body).Member as PropertyInfo;
                var value = property?.GetValue(this, null);

                if (!inputComponent.Input.Isvalid(value) && property != null)
                {
                    errors.Add(property.Name, inputComponent.Input.GetMessage(property.Name));
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

    public class InputComponent<TOwner, TViewModel> where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
    {
        public InputComponent(Expression<Func<TViewModel, object>> expression, BaseInput input, ViewModel<TOwner, TViewModel> viewModel)
        {
            Input = input;
            ViewModel = viewModel;
            Expression = expression;
        }

        public BaseInput Input { get; set; }
        public ViewModel<TOwner, TViewModel> ViewModel { get; set; }
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