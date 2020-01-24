using System;
using System.Collections.Generic;
using System.Windows;

namespace VotoTouch.WPF
{

    // InterClassManager è un framework per la comunicazione interclasse
    //      Versione 1.0 - 20/05/2015
    // attraverso questo è possibile chiamare un metodo con un comando e 2 parametri verso classi
    // indipendentemente dal conoscere o meno cosa fa la classe
    //
    //  I messaggi hanno una stringa di identificazione e 2 parametri object che possono essere usati
    // 
    //  Per utilizzare il framework:
    //  1. La classe "chiamata" deve implementare l'interfaccia IInterClassMessenger, quindi dichiarata come:
    //          class GenericClass : IInterClassMessenger            oppure
    //          class MainWindow : Window, IInterClassMessenger
    //     è possibile applicare l'interfaccia a tutte le classi
    //
    //  2. La classe "chiamata" deve implementare il metodo InterClassCommand che sarà chiamato 
    //     dalle altre classi, dove dovrà processare le stringhe messaggi
    //
    //  3. La classe "chiamata" deve registrare i messaggi a cui potrà rispondere
    //             InterClassManager.RegisterMessage(this, "DO_SOMETHING_MSG");
    //
    //  4. Per chiamare il messaggio, la classe "chiamante" userà:
    //              InterClassManager.NotifyColleaguesSync("DO_SOMETHING_MSG", Parametro1, null)  
    //              InterClassManager.NotifyColleaguesAsync("DO_SOMETHING_MSG", Parametro1, null)  


    // classe interfaccia per comunicazione di comandi interclassi
    interface IInterClassMessenger
    {        
        // metodo che sarà richiamato sulla classe
        void InterClassCommand(string ACommand, object AParam, object WParam, object YParam, object ZParam);
    }

    public class InterClassMessenger
    {
        private delegate void DecoupleCommand(object sender, string message, object AParam, 
                                    object WParam, object YParam, object ZParam);
        private event DecoupleCommand evDecoupleCommand;

        public InterClassMessenger()
        {
            // costruttore

            // metodo di decoupling
            evDecoupleCommand += new DecoupleCommand(onDecoupleCommand);
        }

        public void RegisterClass(object AClass)
        {
            // non serve?   
        }

        public void UnregisterClass(object AClass)
        {
            // non serve?
        }

        public void RegisterMessage(object AClass, string AMessage)
        {
            if (string.IsNullOrEmpty(AMessage))
            {
                throw new ArgumentException("'message' cannot be null or empty.");
            }

            if (AClass == null)
            {
                throw new ArgumentException("'object' cannot be null or empty.");
            }

            _messageToActionsMap.AddAction(AMessage, AClass); 
        }

        public void UnregisterMessage(object AClass, string AMessage)
        {
            // da fare
        }

        // messaggi sincroni, cioè aspetta la fine esecuzione per mandarne un altro
        public void NotifyColleaguesSync(string message, object AParam, object WParam = null, 
            object YParam = null, object ZParam = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("'message' cannot be null or empty.");
            }

            var actionObjs = _messageToActionsMap.GetActions(message);

            if (actionObjs != null)
            {        
                // listo sugli oggetti
                foreach (object actionObj in actionObjs)
                {
                    if ((actionObj as IInterClassMessenger) != null)
                    {
                        (actionObj as IInterClassMessenger).InterClassCommand(message, AParam, WParam, YParam, ZParam);
                    }
                }
            }
        }

        // messaggi asincroni, cioè viene mandato il messaggio senza aspettare che 
        // venga eseguito, ma sullo stesso thread
        public void NotifyColleaguesAsync(string message, object AParam, object WParam = null, 
            object YParam = null, object ZParam = null)
        {
            // da fare metodo asincrono
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("'message' cannot be null or empty.");
            }

            var actionObjs = _messageToActionsMap.GetActions(message);

            if (actionObjs != null)
            {
                // listo sugli oggetti
                foreach (object actionObj in actionObjs)
                {
                    GoDecoupleCommand(actionObj, message, AParam, WParam, YParam, ZParam);
                }
            }
        }

        private void GoDecoupleCommand(object target, string message, object AParam, 
                    object WParam, object YParam, object ZParam)
        {
            Application.Current.Dispatcher.BeginInvoke(evDecoupleCommand, 
                    new object[] { target, message, AParam, WParam, YParam, ZParam });            
        }

        private void onDecoupleCommand(object target, string message, object AParam, object WParam, 
            object YParam, object ZParam)
        {
            if ((target as IInterClassMessenger) != null)
            {
                (target as IInterClassMessenger).InterClassCommand(message, AParam, WParam, YParam, ZParam);
            }
        }


        #region MessageToActionsMap [nested class]
        /// <summary> 
        /// This class is an implementation detail of the Messenger class. 
        /// </summary> 
        private class MessageToActionsMap
        {
            // Stores a hash where the key is the message and the value is the list of object to invoke. 

            readonly Dictionary<string, List<WeakAction>> _map = new Dictionary<string, List<WeakAction>>();
            internal MessageToActionsMap()
            {
            }

            /// <summary> 
            /// Adds an action to the list. 
            /// </summary> 
            /// <param name="message">The message to register.</param> 
            /// <param name="target">The target object to invoke, or null.</param> 

            internal void AddAction(string message, object target)
            {
                if (message == null)
                {
                    throw new ArgumentNullException("message");
                }

                //if (method == null)
                //{
                //    throw new ArgumentNullException("method");
                //}

                lock (_map)
                {
                    if (!_map.ContainsKey(message))
                    {
                        _map[message] = new List<WeakAction>();
                    }

                    _map[message].Add(new WeakAction(target));
                }
            }

            /// <summary> 
            /// Gets the list of object to be invoked for the specified message 
            /// </summary> 
            /// <param name="message">The message to get the actions for</param> 
            /// <returns>Returns a list of actions that are registered to the specified message</returns> 
            internal List<object> GetActions(string message)
            {

                if (message == null)
                {
                    throw new ArgumentNullException("message");
                }

                List<object> actions = null;

                lock (_map)
                {
                    if (!_map.ContainsKey(message))
                    {
                        return null;
                    }

                    List<WeakAction> weakActions = _map[message];
                    actions = new List<object>(weakActions.Count);


                    for (int i = weakActions.Count - 1; i >= -1 + 1; i += -1)
                    {
                        WeakAction weakAction = weakActions[i];

                        if (weakAction == null)
                        {
                            continue;
                        }

                        object action = weakAction.CreateAction();

                        if (action != null)
                        {
                            actions.Add(action);
                        }
                        else
                        {
                            // The target object is dead, so get rid of the weak action. 
                            weakActions.Remove(weakAction);
                        }
                    }

                    // Delete the list from the map if it is now empty. 
                    if (weakActions.Count == 0)
                    {
                        _map.Remove(message);
                    }

                }
                return actions;
            }
        }
        #endregion

        #region WeakAction [nested class]
        /// <summary> 
        /// This class is an implementation detail of the MessageToActionsMap class. 
        /// </summary> 
        private class WeakAction
        {
            //readonly Type _delegateType;
            //readonly MethodInfo _method;

            readonly WeakReference _targetRef;
            /// <summary> 
            /// Constructs a WeakAction. 
            /// </summary> 
            /// <param name="target">The object on which the target method is invoked, or null if the method is static.</param> 

            internal WeakAction(object target)
            {
                if (target == null)
                {
                    _targetRef = null;

                }
                else
                {
                    _targetRef = new WeakReference(target, true);
                }

                //_method = method;

                //if (parameterType == null)
                //{
                //    _delegateType = typeof(Action);

                //}
                //else
                //{
                //    _delegateType = typeof(Action<>).MakeGenericType(parameterType);
                //}
            }

            /// <summary> 
            /// Creates a "throw away" delegate to invoke the method on the target, or null if the target object is dead. 
            /// </summary> 
            internal object CreateAction()
            {
                // Rehydrate into a real Action object, so that the method can be invoked. 
                if (_targetRef == null)
                {
                    return null; // Delegate.CreateDelegate(_delegateType, _method);
                }
                else
                {
                    try
                    {
                        object target = _targetRef.Target;

                        if (target != null)
                        {
                            return target; // Delegate.CreateDelegate(_delegateType, target, _method);
                        }

                    }
                    catch
                    {
                    }

                }

                return null;
            }
        }
        #endregion

        #region Fields
        readonly MessageToActionsMap _messageToActionsMap = new MessageToActionsMap();
        #endregion

    }
}
