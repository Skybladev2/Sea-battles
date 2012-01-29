using System;
using System.Collections.Generic;
using System.Text;

namespace SeaBattles
{
    /// <summary>
    /// Попытка замены атрибутов отражения с целью будущей кроссплатформенности.
    /// </summary>
    internal interface IReflectionAttributes
    {
        /// <summary>
        /// Устанавливает значение атрибута, если он уже добавлен, или добавляет новый атрибут с указанным значением.
        /// </summary>
        /// <param name="attributeName">Название атрибута.</param>
        /// <param name="attributeValue">Значение атрибута.</param>
        /// <returns>Успешность добавления/изменения атрибута.</returns>
        bool SetAttribute(string attributeName, string attributeValue);

        /// <summary>
        /// Получает значение указанного атрибута.
        /// </summary>
        /// <param name="attributeName">Название извлекаемого атрибута.</param>
        /// <param name="attributeValue">Значение атрибута.</param>
        /// <returns>true, если атрибут успешно извлечён, false, если не удалось его извлечь (например, такого атрибута нет).</returns>
        bool GetAttribute(string attributeName, out string attributeValue);

        /// <summary>
        /// Удаляет заданный атрибут у объекта.
        /// </summary>
        /// <param name="attributeName">Название удаляемого атрибута.</param>
        void RemoveAttribute(string attributeName);
    }
}