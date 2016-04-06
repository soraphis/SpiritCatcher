/**
Copyright (c) <2015>, <Devon Klompmaker>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
**/

namespace uState
{
    [System.Serializable]
    public class TransitionCondition
    {
        public string propertyName;
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public ITransitionNumericCompareType numericCompareType;
        public ITransitionBoolCompareType boolCompareType;
        public ITransitionGameObjectCompareType gameObjectCompareType;

        public bool DoesPassCondition(Property property)
        {
            switch(property.propertyType)
            {
                case IPropertyType.BOOL:
                    return DoesPassBoolCondition(property);
                case IPropertyType.FLOAT:
                    return DoesPassFloatCondition(property);
                case IPropertyType.INT:
                    return DoesPassIntCondition(property);
                case IPropertyType.GAMEOBJECT:
                    return DoesPassGameObjectCondition(property);
            }

            return true;
        }

        private bool DoesPassBoolCondition(Property property)
        {
            switch (boolCompareType)
            {
                case ITransitionBoolCompareType.Equal:
                    return property.boolValue == boolValue;
                case ITransitionBoolCompareType.NotEqual:
                    return property.boolValue != boolValue;
            }

            return true;
        }

        private bool DoesPassFloatCondition(Property property)
        {
            switch(numericCompareType)
            {
                case ITransitionNumericCompareType.Equals:
                    return property.floatValue == floatValue;
                case ITransitionNumericCompareType.GreaterThan:
                    return property.floatValue > floatValue;
                case ITransitionNumericCompareType.GreaterThanOrEquals:
                    return property.floatValue >= floatValue;
                case ITransitionNumericCompareType.LesserThan:
                    return property.floatValue < floatValue;
                case ITransitionNumericCompareType.LesserThanOrEquals:
                    return property.floatValue <= floatValue;
            }

            return true;
        }

        private bool DoesPassIntCondition(Property property)
        {
            switch (numericCompareType)
            {
                case ITransitionNumericCompareType.Equals:
                    return property.intValue == intValue;
                case ITransitionNumericCompareType.GreaterThan:
                    return property.intValue > intValue;
                case ITransitionNumericCompareType.GreaterThanOrEquals:
                    return property.intValue >= intValue;
                case ITransitionNumericCompareType.LesserThan:
                    return property.intValue < intValue;
                case ITransitionNumericCompareType.LesserThanOrEquals:
                    return property.intValue <= intValue;
            }

            return true;
        }

        private bool DoesPassGameObjectCondition(Property property)
        {
            switch (gameObjectCompareType)
            {
                case ITransitionGameObjectCompareType.IsNull:
                    if(boolValue)
                    {
                        return property.gameObjectValue == null;
                    }
                    else
                    {
                        return property.gameObjectValue != null;
                    }
            }

            return true;
        }
    }

    public enum ITransitionNumericCompareType
    {
        GreaterThan,
        GreaterThanOrEquals,
        LesserThan,
        LesserThanOrEquals,
        Equals
    }

    public enum ITransitionBoolCompareType
    {
        Equal,
        NotEqual
    }

    public enum ITransitionGameObjectCompareType
    {
        IsNull
    }
}
