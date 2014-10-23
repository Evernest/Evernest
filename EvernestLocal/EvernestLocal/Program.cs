using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

/* Added references */
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace EvernestLocal
{
    enum Action
    {
        ActionConnexion,
        ActionPush,
        ActionPull,
        ActionQuit,
        ActionNone
    };

    internal class Program
    {

        private static void Main(string[] args)
        {
            ApiUrl apiUrl = loadUrl("API_Parameters.json");
            Account account = loadAccount("API_Account.json");

            Console.SetWindowSize(100, 30);
            Write(13,2,"Bienvenue dans Evernest Local qui vous permet de tester l'API Evernest.");

            Boolean getToken = false;
            string token = null;
            string line;

            while (true)
            {
                Write(65,4,"Identification : " + getToken + "   ");
                Action action = chooseAction(getToken);
                switch (action)
                {
                    case Action.ActionConnexion:
                        ClearConsole();
                        Write(2,10,"Tentative de connexion avec les identifiants du fichiers : API_Account.json");
                        Console.SetCursorPosition(2, 11);
                        account.ToPrint();
                        AccountResponse a = Connexion(apiUrl.GetConnexion(), account);
                        Write(2,12,"Récupération du token d'identification.");
                        Console.SetCursorPosition(2, 13);
                        a.ToPrint();
                        token = a.new_token;
                        getToken = true;
                        ReturnChooseActionLocation();
                        break;
                    case Action.ActionPush:
                        if (!getToken)
                        {
                            Write(2, 10, "Vous devez vous identifier.");
                            ReturnChooseActionLocation();
                            continue;
                        }
                        getToken = false;
                        ClearConsole();
                        int compteur = 0;
                        StreamReader eventFile = new StreamReader(@"..\\..\\Evenements.txt");
                        while ((line = eventFile.ReadLine()) != null)
                        {
                            Write(2,10,"Tentative d'ajout de l'évènement suivant : ");
                            Write(2, 11, line);
                            Request req = new Request(line, token);
                            PushEventResponse r = Push(apiUrl.GetPush(), req);
                            Write(2, 12, "Récupération du de l'identifiant de l'évènement.");
                            token = r.new_token;
                            getToken = true;
                            Console.SetCursorPosition(2, 13);
                            r.ToPrint();
                            Write(2,14,"Nombre d'évènement ajouté : " + compteur);
                            ++compteur;
                        }
                        Write(2, 16, "Opération terminée.");
                        ReturnChooseActionLocation();
                        break;
                    case Action.ActionPull:
                        if (!getToken)
                        {
                            Write(2, 10, "Vous devez vous identifier.");
                            ReturnChooseActionLocation();
                            continue;
                        }
                        getToken = false;
                        ClearConsole();
                        Write(2,10,"Choisir l'indice de début : (-1 pour random)");
                        Console.SetCursorPosition(2, 11);
                        int id1 = int.Parse(Console.ReadLine());
                        Write(2,12,"Choisir l'indice de fin : (-1 pour random)");
                        Console.SetCursorPosition(2, 13);
                        int id2 = int.Parse(Console.ReadLine());
                        Write(2, 14, "Tentative de récupération d'évènement : ");
                        string url;
                        if (id1 == -1 && id2 == -1)
                            url = apiUrl.GetPull();
                        else
                            url = apiUrl.GetPull(id1,id2);
                        RequestResponse per = Pull(url, new RequestToken(token));
                        token = per.new_token;
                        getToken = true;
                        Console.SetCursorPosition(2, 16);
                        per.ToPrint();   
                        break;
                    case Action.ActionQuit:
                        ClearConsole();
                        Write(2,28,"Au revoir. ");
                        System.Environment.Exit(0);
                        break;
                    case Action.ActionNone:
                        break;
                }
            }
        }

        private static AccountResponse Connexion(string url, Account account)
        {
            HttpClient request = new HttpClient(url);
            request.SendData(ObjectToJson(account));
            return JsonToObject<AccountResponse>(request.GetResponse());
        }

        private static PushEventResponse Push(string url, Request r)
        {
            HttpClient request = new HttpClient(url);
            request.SendData(ObjectToJson(r));
            return JsonToObject<PushEventResponse>(request.GetResponse());
        }

        private static RequestResponse Pull(string url, RequestToken r)
        {
            HttpClient request = new HttpClient(url);
            request.SendData(ObjectToJson(r));
            return JsonToObject<RequestResponse>(request.GetResponse());
        }

        private static Action chooseAction(Boolean token)
        {
            Write(2,4,"Choisir une action : ");
            Write(8,5,"1 - Connexion");
            if (!token)
                Console.ForegroundColor = ConsoleColor.DarkGray;
            Write(8,6,"2 - Push");
            Write(8,7,"3 - Pull");
            Console.ResetColor();
            Write(8,8,"4 - Quit");
            ReturnChooseActionLocation();

            ConsoleKeyInfo input = Console.ReadKey();

            if (!token && input.KeyChar != '4' && input.KeyChar != '1')
            {
                Write(2,10,"Vous devez vous identifier.");
                ReturnChooseActionLocation();
                return Action.ActionNone;
            }
                
            switch (input.KeyChar)
            {
                case '1':
                    return Action.ActionConnexion;
                case '2':
                    return Action.ActionPush;
                case '3':
                    return Action.ActionPull;
                case '4':
                    return Action.ActionQuit;
                default:
                    return Action.ActionNone;
            }
        }

        private static Account loadAccount(string account_file)
        {
            string contentFile = File.ReadAllText(@"..\\..\\"+account_file);   
            return JsonToObject<Account>(contentFile);
        }

        private static ApiUrl loadUrl(string param_file)
        {
            string contentFile = File.ReadAllText(@"..\\..\\"+param_file);   
            return JsonToObject<ApiUrl>(contentFile);
        }

        private static T JsonToObject<T>(string data)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            T res = jsonSerializer.Deserialize<T>(data);
            return res;
        }

        private static string ObjectToJson(object obj)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            return jsonSerializer.Serialize(obj);
        }

        private static void Write(int col, int row, string text)
        {
            Console.SetCursorPosition(col, row);
            Console.Write(text);
        }

        private static void ClearConsole()
        {
            int i;
            for (i = 10; i < 29; ++i)
                Write(1, i, "                                                                                                    ");
        }

        private static void ReturnChooseActionLocation()
        {
            Console.SetCursorPosition(23, 4);
        }
    }
}
