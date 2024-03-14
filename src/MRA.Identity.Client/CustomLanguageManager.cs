namespace MRA.Identity.Client;

public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
{
    public CustomLanguageManager()
    {
        AddTranslation("ru", "EmailValidator", "Неверный email адрес.");
        AddTranslation("ru", "GreaterThanOrEqualValidator", "Должно быть больше или равно '{ComparisonValue}'.");
        AddTranslation("ru", "GreaterThanValidator", "Должно быть больше '{ComparisonValue}'.");
        AddTranslation("ru", "LengthValidator", "Должно быть длиной от {MinLength} до {MaxLength} символов. Количество введенных символов: {TotalLength}.");
        AddTranslation("ru", "MinimumLengthValidator", "Должно быть длиной не менее {MinLength} символов. Количество введенных символов: {TotalLength}.");
        AddTranslation("ru", "MaximumLengthValidator", "Должно быть длиной не более {MaxLength} символов. Количество введенных символов: {TotalLength}.");
        AddTranslation("ru", "LessThanOrEqualValidator", "Должно быть меньше или равно '{ComparisonValue}'.");
        AddTranslation("ru", "LessThanValidator", "Должно быть меньше '{ComparisonValue}'.");
        AddTranslation("ru", "NotEmptyValidator", "Должно быть заполнено.");
        AddTranslation("ru", "NotEqualValidator", "Не должно быть равно '{ComparisonValue}'.");
        AddTranslation("ru", "NotNullValidator", "Должно быть заполнено.");
        AddTranslation("ru", "PredicateValidator", "Не выполнено указанное условие.");
        AddTranslation("ru", "AsyncPredicateValidator", "Не выполнено указанное условие.");
        AddTranslation("ru", "RegularExpressionValidator", "Имеет неверный формат.");
        AddTranslation("ru", "EqualValidator", "Должно быть равно '{ComparisonValue}'.");
        AddTranslation("ru", "ExactLengthValidator", "Должно быть длиной {MaxLength} символа(ов). Количество введенных символов: {TotalLength}.");
        AddTranslation("ru", "InclusiveBetweenValidator", "Должно быть в диапазоне от {From} до {To}. Введенное значение: {PropertyValue}.");
        AddTranslation("ru", "ExclusiveBetweenValidator", "Должно быть в диапазоне от {From} до {To} (не включая эти значения). Введенное значение: {PropertyValue}.");
        AddTranslation("ru", "CreditCardValidator", "Неверный номер карты.");
        AddTranslation("ru", "ScalePrecisionValidator", "Должно содержать не более {ExpectedPrecision} цифр всего, в том числе {ExpectedScale} десятичных знака(ов). Введенное значение содержит {Digits} цифр(ы) в целой части и {ActualScale} десятичных знака(ов).");
        AddTranslation("ru", "EmptyValidator", "Должно быть пустым.");
        AddTranslation("ru", "NullValidator", "Должно быть пустым.");
        AddTranslation("ru", "EnumValidator", "Содержит недопустимое значение '{PropertyValue}'.");
        AddTranslation("ru", "Length_Simple", "Должно быть длиной от {MinLength} до {MaxLength} символов.");
        AddTranslation("ru", "MinimumLength_Simple", "Должно быть длиной не менее {MinLength} символов.");
        AddTranslation("ru", "MaximumLength_Simple", "Должно быть длиной не более {MaxLength} символов.");
        AddTranslation("ru", "ExactLength_Simple", "Должно быть длиной {MaxLength} символа(ов).");
        AddTranslation("ru", "InclusiveBetween_Simple", "Должно быть в диапазоне от {From} до {To}.");
    }
}
