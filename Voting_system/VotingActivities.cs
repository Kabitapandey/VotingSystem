using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voting_system
{
    class VotingActivities
    {
        public string voterListFilePath = "C:\\Users\\pande\\Desktop\\voterList.json";
        public string candidateFilePath = "C:\\Users\\pande\\Desktop\\candidateList.json";
        public void DisplayOptionsForVoters(List<VoterModule> data)
        {
            Console.WriteLine("Enter the option you like to choose:");
            Console.WriteLine("1: Add vote");
            Console.WriteLine("2: Edit vote");
            Console.WriteLine("3: Delete vote");
            Console.WriteLine("4: See voting details");

            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    AddVote(data);
                    break;
                case 2:
                    Console.WriteLine("Enter your voter id:");
                    string voterId = Console.ReadLine();
                    Console.WriteLine("Enter the candidate no you want to vote:");
                    VotingModule vm = new VotingModule();
                    Console.WriteLine("1: {0}", vm.CandidateName1);
                    Console.WriteLine("2: {0}", vm.CandidateName2);
                    int candidate = int.Parse(Console.ReadLine());

                    UpdateVote(voterId, candidate);
                    break;
                case 3:
                    DeleteVote();
                    break;
                case 4:
                    GetTotalVotes();
                    break;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }

        /*adding votes function*/
        public void AddVote(List<VoterModule> voterData)
        {
            VotingModule vm = new VotingModule();
            Console.WriteLine("Enter your voting Id");
            string voterId = Console.ReadLine();

            Console.WriteLine("Which candidate you want to vote?");
            Console.WriteLine("1: {0}", vm.CandidateName1);
            Console.WriteLine("2: {0}", vm.CandidateName2);

            int candidate = int.Parse(Console.ReadLine());



            if (File.Exists(voterListFilePath))
            {

                string votings = File.ReadAllText(voterListFilePath);
                if (votings != null)
                {
                    voterData = JsonSerializer.Deserialize<List<VoterModule>>(votings);
                }
                foreach (VoterModule voter in voterData)
                {
                    if (voter.VoterId == voterId)
                    {
                        Console.WriteLine("You have already voted!!Do you want to update your vote...yes or no??");
                        string voterDecision = Console.ReadLine();

                        if (voterDecision == "yes")
                        {
                            UpdateVote(voterId, candidate);
                            return;
                        }
                        else if (voterDecision == "no")
                        {
                            Console.WriteLine("Vote has not been modified!!");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("You must answer in yes or                no!!");
                            return;
                        }
                    }
                }
            }

            if (candidate == 1 || candidate == 2)
            {
                voterData.Add(new VoterModule() { VoterId = voterId, Vote = candidate });

            }
            StreamWriter voterStremWriter = new StreamWriter(voterListFilePath);
            string json = JsonSerializer.Serialize(voterData);
            voterStremWriter.WriteLine(json);
            voterStremWriter.Close();

            if (!File.Exists(candidateFilePath))
            {

                StreamWriter candidateStreamWriters = new StreamWriter(candidateFilePath);
                List<VotingModule> candidates = new List<VotingModule>();
                candidates.Add(new VotingModule()
                {
                    CandidateName1 = "candidate 1",
                    VoteCountCand1 = 0,
                    CandidateName2 = "candidate 2",
                    VoteCountCand2 = 0,
                });

                string candidateData = JsonSerializer.Serialize(candidates);
                candidateStreamWriters.WriteLine(candidateData);
                candidateStreamWriters.Close();
            }

            string candidateStreamReader = File.ReadAllText(candidateFilePath);
            List<VotingModule> data = JsonSerializer.Deserialize<List<VotingModule>>(candidateStreamReader);

            foreach (VotingModule module in data)
            {
                vm.VoteCountCand1 = module.VoteCountCand1;
                vm.VoteCountCand2 = module.VoteCountCand2;
            }


            switch (candidate)
            {
                case 1:
                    vm.VoteCountCand1 += 1;
                    break;
                case 2:
                    vm.VoteCountCand2 += 1;
                    break;
                default:
                    Console.WriteLine("Invalid vote");
                    return;
                    break;
            }


            foreach (VotingModule module in data)
            {
                module.VoteCountCand1 = vm.VoteCountCand1;
                module.VoteCountCand2 = vm.VoteCountCand2;
            }
            StreamWriter candidateStreamWriter = new StreamWriter(candidateFilePath);
            string jsonVoteList = JsonSerializer.Serialize(data);
            candidateStreamWriter.WriteLine(jsonVoteList);
            candidateStreamWriter.Close();

            Console.WriteLine("Your vote has been recorded!!");
        }

        /*function to update votes*/
        public void UpdateVote(string voterId, int candidate)
        {
            List<VoterModule> voterList = new List<VoterModule>();

            string voterFileData = File.ReadAllText(voterListFilePath);
            voterList = JsonSerializer.Deserialize<List<VoterModule>>(voterFileData);

            List<VotingModule> candidateModule = new List<VotingModule>();
            string candidateFileData = File.ReadAllText(candidateFilePath);
            candidateModule = JsonSerializer.Deserialize<List<VotingModule>>(candidateFileData);
            VotingModule vm = new VotingModule();
            vm.VoteCountCand1 = candidateModule[0].VoteCountCand1;
            vm.VoteCountCand2 = candidateModule[0].VoteCountCand2;
            string msg;
            foreach (VoterModule voter in voterList)
            {
                if (voter.VoterId == voterId)
                {
                    if (voter.Vote == 2 && candidate == 1)
                    {
                        vm.VoteCountCand1 += 1;
                        vm.VoteCountCand2 -= 1;

                        StreamWriter candidateWriter = new StreamWriter(candidateFilePath);
                        List<VotingModule> candidateModuleList = new List<VotingModule>()
                        {
                            new VotingModule()
                            {
                                VoteCountCand1=vm.VoteCountCand1,
                                VoteCountCand2=vm.VoteCountCand2
                            }
                        };
                        voter.Vote = candidate;
                        string json = JsonSerializer.Serialize(candidateModuleList);
                        candidateWriter.WriteLine(json);
                        candidateWriter.Close();

                        Console.WriteLine("Your vote has been updated!!");

                    }
                    else if (voter.Vote == 1 && candidate == 2)
                    {
                        vm.VoteCountCand1 -= 1;
                        vm.VoteCountCand2 += 1;

                        StreamWriter candidateWriter = new StreamWriter(candidateFilePath);
                        List<VotingModule> candidateModuleList = new List<VotingModule>()
                        {
                            new VotingModule()
                            {
                                VoteCountCand1=vm.VoteCountCand1,
                                VoteCountCand2=vm.VoteCountCand2
                            }
                        };
                        voter.Vote = candidate;
                        string json = JsonSerializer.Serialize(candidateModuleList);
                        candidateWriter.WriteLine(json);
                        candidateWriter.Close();
                        Console.WriteLine("Your vote has been updated!!");

                    }
                    else if ((voter.Vote == 1 && candidate == 1) || (voter.Vote == 2 && candidate == 2))
                    {
                        Console.WriteLine("You have already voted this candidate!!");

                    }
                    else
                    {
                        Console.WriteLine("Invalid vote");

                    }
                    StreamWriter voterWriter = new StreamWriter(voterListFilePath);
                    string jsonList = JsonSerializer.Serialize(voterList);
                    voterWriter.WriteLine(jsonList);
                    voterWriter.Close();
                    return;
                }

            }
            Console.WriteLine("Your vote hasn't been recorded yet to update!!");
        }

        /*function to delete votes*/
        public void DeleteVote()
        {
            List<VoterModule> voterList = new List<VoterModule>();
            Console.WriteLine("Enter your voter id:");
            string voterId = Console.ReadLine();
            string voterFileData = File.ReadAllText(voterListFilePath);
            voterList = JsonSerializer.Deserialize<List<VoterModule>>(voterFileData);
            List<VotingModule> candidateModule = new List<VotingModule>();
            string candidatesVotes = File.ReadAllText(candidateFilePath);
            candidateModule = JsonSerializer.Deserialize<List<VotingModule>>(candidatesVotes);
            VotingModule vm = new VotingModule();
            vm.VoteCountCand1 = candidateModule[0].VoteCountCand1;
            vm.VoteCountCand2 = candidateModule[0].VoteCountCand2;

            foreach (VoterModule voter in voterList)
            {
                if (voter.VoterId == voterId)
                {
                    voterList.Remove(voter);

                    if (voter.Vote == 1)
                    {
                        vm.VoteCountCand1 -= 1;
                        StreamWriter streamWriter = new StreamWriter(voterListFilePath);
                        string json = JsonSerializer.Serialize(voterList);
                        streamWriter.WriteLine(json);
                        streamWriter.Close();

                        StreamWriter candidateWriter = new StreamWriter(candidateFilePath);
                        List<VotingModule> candidateList = new List<VotingModule> { new VotingModule() { VoteCountCand1 = vm.VoteCountCand1, VoteCountCand2 = vm.VoteCountCand2 } };

                        string data = JsonSerializer.Serialize(candidateList);
                        candidateWriter.WriteLine(data);
                        candidateWriter.Close();

                        Console.WriteLine("Your vote has been deleted successfully!!");
                        return;
                    }
                    else if (voter.Vote == 2)
                    {
                        vm.VoteCountCand2 -= 1;
                        StreamWriter streamWriter = new StreamWriter(voterListFilePath);
                        string json = JsonSerializer.Serialize(voterList);
                        streamWriter.WriteLine(json);
                        streamWriter.Close();

                        StreamWriter candidateWriter = new StreamWriter(candidateFilePath);
                        List<VotingModule> candidateList = new List<VotingModule> { new VotingModule() { VoteCountCand1 = vm.VoteCountCand1, VoteCountCand2 = vm.VoteCountCand2 } };

                        string data = JsonSerializer.Serialize(candidateList);
                        candidateWriter.WriteLine(data);
                        candidateWriter.Close();

                        Console.WriteLine("Your vote has been deleted successfully!!");
                        return;
                    }

                }

            }
            Console.WriteLine("You haven't voted yet to delete your vote!!");
        }
        /*seeing total votes*/
        public void GetTotalVotes()
        {
            List<VotingModule> votingModule = new List<VotingModule>();
            string candidateFileData = File.ReadAllText(candidateFilePath);

            votingModule = JsonSerializer.Deserialize<List<VotingModule>>(candidateFileData);
            VotingModule vm = new VotingModule();
            vm.VoteCountCand1 = votingModule[0].VoteCountCand1;
            vm.VoteCountCand2 = votingModule[0].VoteCountCand2;

            int totalVotes = vm.VoteCountCand1 + vm.VoteCountCand2;

            float voteCount1 = ((float)vm.VoteCountCand1 / (float)totalVotes) * 100;
            float voteCount2 = ((float)vm.VoteCountCand2 / (float)totalVotes) * 100;

            Console.WriteLine("Total vote of candidate1:");
            Console.WriteLine(voteCount1 + "%");

            Console.WriteLine("Total vote of candidate2:");
            Console.WriteLine(voteCount2 + "%");

        }
    }
}
