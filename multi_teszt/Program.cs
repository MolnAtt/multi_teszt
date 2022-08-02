using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace multi_teszt
{
	class Program
	{
		static void Main(string[] args)
		{
			Robot karesz = new Robot("Karesz");
			Robot lilesz = new Robot("Lilesz");
			Robot miska = new Robot("Miska");

			karesz.feladat = delegate () 
			{
				karesz.Csinál_valamit();
			};

			lilesz.feladat = delegate ()
			{
				for (int i = 0; i < 3; i++)
				{
					lilesz.Csinál_valamit();
				}
			};

			miska.feladat = delegate ()
			{
				for (int i = 0; i < 7; i++)
				{
					miska.Csinál_valamit();
				}
			};

			Robot.timeout = 10;

			Console.WriteLine("-------------------------------------------------------");
			Console.WriteLine($"JÁTÉKMESTER: elindítom a játékot");

			Robot.játék_elindítása();
			Console.WriteLine("-------------------------------------------------------");
			Console.WriteLine($"JÁTÉKMESTER: Vége a játéknak");
			Console.ReadKey();

		}
	}


	class Robot
	{
		// instancia tulajdonságai
		public string nev;
		public Thread thread;

		public Robot rákövetkezője, megelőzője;
		public bool kész;
		public bool vár;
		public static int játékban_lévők_száma;

		public Action feladat;

		// string-reprezentáció debughoz
		public override string ToString() => this.nev;

		// osztály tulajdonságai
		public static Random r = new Random();
		private static Robot kezdő_robot;

		


		static readonly int rmin = 3;
		static readonly int rmax = 10;
		public static int timeout;
		static int counter = 0;

		static Dictionary<Robot, int> pálya = new Dictionary<Robot, int>(); 

		// konstruktor
		public Robot(string nev)
		{
			this.nev = nev;
			this.thread = new Thread(new ThreadStart(FELADAT_BUROK));
			this.kész = false;
			this.vár = false;

			if (Robot.kezdő_robot == null)
				Robot.kezdő_robot = this;

			this.Végére_fűz();

			pálya[this] = 0;

			Console.WriteLine($"{this.nev} létrejött");
		}

		#region A lánc adatszerkezet metódusai

		public void Beszúr_ez_elé(Robot ez)
		{
			this.rákövetkezője = ez;
			this.megelőzője = ez.megelőzője;
			this.rákövetkezője.megelőzője = this;
			this.megelőzője.rákövetkezője = this;
			Robot.játékban_lévők_száma++;
		}
		/// <summary>
		/// ha már csak egyelemű a lista, akkor hatástalan.
		/// </summary>
		public void Kifűz()
		{
			if (this == Robot.kezdő_robot)
			{
				Robot.kezdő_robot = rákövetkezője;
			}
			this.rákövetkezője.megelőzője = this.megelőzője;
			this.megelőzője.rákövetkezője = this.rákövetkezője;
			Robot.játékban_lévők_száma--;
		}
		public void Végére_fűz() => Beszúr_ez_elé(Robot.kezdő_robot);

		#endregion

		#region A robot kezelésének metódusai
		/// <summary>
		/// ez az, amit futtat majd a thread
		/// </summary>
		void FELADAT_BUROK()
		{
			this.feladat();
			this.kész = true;
			Console.WriteLine($"{nev}: KÉSZEN VAGYOK!");
			this.Kiszállok();
		}

		private void Kiszállok()
		{
			Robot ki_fog_jönni = rákövetkezője;
			this.Kifűz();
			ki_fog_jönni.Te_jössz();
		}

		/// <summary>
		/// ez az, amit a user szerkeszt
		/// </summary>
		
		public void Csinál_valamit()
		{
//			Console.WriteLine($"=============== SZÜNET ================= {nev} körében");
			if (this==Robot.kezdő_robot)
			{
				Console.WriteLine($"===================\n Letelt egy nagykör!");
			}
			Thread.Sleep(500);
			pálya[this]++;
			Console.WriteLine($"-------------------\n{nev} lép.");
			foreach (Robot r in pálya.Keys)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write($"{r.nev}: {pálya[r]}\t");
				Console.ForegroundColor = ConsoleColor.White;
			}
			Console.WriteLine();
			if (1 < Robot.játékban_lévők_száma)
			{
				Letelt_a_köröd();
			}
		}

		#endregion

		#region A láncba fűzött szálak indítgatásának metódusai
		/// <summary>
		/// a játékosok láncán elkezd végighaladni a léptetés. 
		/// Aki kész van, kiesik a láncból. 
		/// Addig megy, míg mindenki kész nem lesz. 
		/// </summary>
		public static void játék_elindítása()
		{
			counter = 0;
			Console.WriteLine("Nyomj egy gombot a kezdéshez!");
			Console.ReadKey();
			kezdő_robot.Te_jössz();
			Várakozik_amig_mindenki_kesz_nem_lesz();
			Console.WriteLine("JÁTÉKMESTER: Vége, mindenki lelépett mindent, vagy lejárt az idő.");
		}

		static void Várakozik_amig_mindenki_kesz_nem_lesz()
		{
			while (Valaki_még_dolgozik() && counter<timeout)
			{
				counter++;
			//	Console.WriteLine($"JÁTÉKMESTER: körbenézek, mi van. ({++counter})");
				Thread.Sleep(1000);
			}
			if (counter < timeout)
			{
				Console.WriteLine($"JÁTÉKMESTER: már nem dolgozik senki");
			}
			else
			{
				Console.WriteLine($"JÁTÉKMESTER: elérte a counter ({counter}) a timeout-ot ({timeout})");
			}
		}

		static bool Valaki_még_dolgozik()
		{
			if (!kezdő_robot.kész)
			{
				Console.WriteLine($"JÁTÉKMESTER: {kezdő_robot} még dolgozik");
				return true;
			}
			Robot aktuális_robot = kezdő_robot.rákövetkezője;
			while (aktuális_robot != kezdő_robot)
			{
				if (!aktuális_robot.kész)
				{
					Console.WriteLine($"JÁTÉKMESTER: {aktuális_robot} még dolgozik");
					return true;
				}
				aktuális_robot = aktuális_robot.rákövetkezője;
			}
			Console.WriteLine("Mindenki készen van.");
			return false;
		}

		void Te_jössz()
		{
			Console.WriteLine($"{nev}: én jövök.");

			if (!kész)
			{
				// Console.WriteLine($"{nev}: nem vagyok kész, szóval indítok.");
				Start_or_Resume();
			}
		}
		void Letelt_a_köröd()
		{
			this.vár = true;
			Console.WriteLine($"{nev}: várakozom, mindjárt felfüggesztem magam, de előbb szólok {rákövetkezője}-nek, hogy ő jön.");
			rákövetkezője.Te_jössz();

			Console.WriteLine($"{nev}: és most felfüggesztem magam");
			this.thread.Suspend();

			// valójában itt kezdődnek a körök!
			Console.WriteLine($"{nev}: Elindítottak.");
			Thread.Sleep(100); // Ez arra kell, hogy amikor megkapja a körét, akkor hagyjon egy kis időt az előzőnek, hogy felfüggessze magát. 

		}
		void Start_or_Resume()
		{
			if (vár)
			{
			//	Console.WriteLine($"{nev}: vártam, szóval folytatom (resume).");
				thread.Resume();
			}
			else
			{
			//	Console.WriteLine($"{nev}: nem vártam, szóval startolok.");
				thread.Start();
			}
		}

		#endregion

		// metódusok


	}
}
