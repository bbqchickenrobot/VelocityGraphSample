// This sample is a preview of VelocityGraph, help us define the best possible graph api! The start of this api is inspired by Dex, see http://www.sparsity-technologies.com/dex.php,
// the sample here mimics what the dex sample is doing. Dex is considered by some to have the best performing graph api currently available. Unlike Dex, which is a C++/C implemented api,
// VelocityGraph is all implemented in C# and enable any type of Element values. VelocityGraph is provided as open source on GitHub, https://github.com/VelocityDB/VelocityGraph. 
// Anyone is welcome to contribute! VelocityGraph is built on top of VelocityDB.
// You will need a VelocityDB license to use VelocityGraph. Eventually VelocityGraph may have its own web site, http://www.VelocityGraph.com/

using System;
using System.IO;
using System.Linq;
using VelocityDb.Session;
using VelocityGraph;
using Element = System.Int64;
using PropertyTypeId = System.Int32;
using PropertyId = System.Int32;
using TypeId = System.Int32;

namespace VelocityGraphSample
{
  class VelocityGraphSample
  {
    static readonly string systemDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
      "VelocityDB" + Path.DirectorySeparatorChar + "Databases" + Path.DirectorySeparatorChar + "VelocityGraphSample");

    static void Main(string[] args)
    {
      if (Directory.Exists(systemDir))
        Directory.Delete(systemDir, true); // remove systemDir from prior runs and all its databases.
      UInt64 graphId;
      using (SessionNoServer session = new SessionNoServer(systemDir, false))
      {
        session.BeginUpdate();
        Graph g = new Graph(session);

    // SCHEMA
        // Add a node type for the movies, with a unique identifier and two indexed Propertys
        TypeId movieType = g.NewNodeType("MOVIE");
        TypeId movieIdType = g.NewProperty(movieType, "ID", DataType.Long, PropertyKind.Unique);
        PropertyId movieTitleType = g.NewProperty(movieType, "TITLE", DataType.String, PropertyKind.Indexed);
        PropertyId movieYearType = g.NewProperty(movieType, "YEAR", DataType.Integer, PropertyKind.Indexed);

        // Add a node type for the people, with a unique identifier and an indexed Property
        TypeId peopleType = g.NewNodeType("PEOPLE");
        PropertyId peopleIdType = g.NewProperty(peopleType, "ID", DataType.Long, PropertyKind.Unique);
        PropertyId peopleNameType = g.NewProperty(peopleType, "NAME", DataType.String, PropertyKind.Indexed);

        // Add an undirected edge type with an Property for the cast of a movie
        TypeId castType = g.NewEdgeType("CAST", false, false);
        PropertyId castCharacterType = g.NewProperty(castType, "CHARACTER", DataType.String, PropertyKind.Basic);

        // Add a directed edge type restricted to go from people to movie for the director of a movie
        TypeId directsType = g.NewRestrictedEdgeType("DIRECTS", peopleType, movieType, false);

    // DATA
        // Add some MOVIE nodes

        Element mLostInTranslation = g.NewNode(movieType);
        g.SetProperty(mLostInTranslation, movieIdType, (long) 1);
        g.SetProperty(mLostInTranslation, movieTitleType, "Lost in Translation");
        g.SetProperty(mLostInTranslation, movieYearType, (int) 2003);

        Element mVickyCB = g.NewNode(movieType);
        g.SetProperty(mVickyCB, movieIdType, (long) 2);
        g.SetProperty(mVickyCB, movieTitleType, "Vicky Cristina Barcelona");
        g.SetProperty(mVickyCB, movieYearType, (int) 2008);

        Element mManhattan = g.NewNode(movieType);
        g.SetProperty(mManhattan, movieIdType, (long) 3);
        g.SetProperty(mManhattan, movieTitleType, "Manhattan");
        g.SetProperty(mManhattan, movieYearType, (int) 1979);


        // Add some PEOPLE nodes
        Element pScarlett = g.NewNode(peopleType);
        g.SetProperty(pScarlett, peopleIdType, (long) 1);
        g.SetProperty(pScarlett, peopleNameType, "Scarlett Johansson");

        Element pBill = g.NewNode(peopleType);
        g.SetProperty(pBill, peopleIdType, (long) 2);
        g.SetProperty(pBill, peopleNameType, "Bill Murray");

        Element pSofia = g.NewNode(peopleType);
        g.SetProperty(pSofia, peopleIdType, (long) 3);
        g.SetProperty(pSofia, peopleNameType, "Sofia Coppola");

        Element pWoody = g.NewNode(peopleType);
        g.SetProperty(pWoody, peopleIdType, (long) 4);
        g.SetProperty(pWoody, peopleNameType, "Woody Allen");

        Element pPenelope = g.NewNode(peopleType);
        g.SetProperty(pPenelope, peopleIdType, (long) 5);
        g.SetProperty(pPenelope, peopleNameType, "Penélope Cruz");

        Element pDiane = g.NewNode(peopleType);
        g.SetProperty(pDiane, peopleIdType, (long) 6);
        g.SetProperty(pDiane, peopleNameType, "Diane Keaton");

        // Add some CAST edges
        Element anEdge;
        anEdge = g.NewEdge(castType, mLostInTranslation, pScarlett);
        g.SetProperty(anEdge, castCharacterType, "Charlotte");

        anEdge = g.NewEdge(castType, mLostInTranslation, pBill);
        g.SetProperty(anEdge, castCharacterType, "Bob Harris");

        anEdge = g.NewEdge(castType, mVickyCB, pScarlett);
        g.SetProperty(anEdge, castCharacterType, "Cristina");

        anEdge = g.NewEdge(castType, mVickyCB, pPenelope);
        g.SetProperty(anEdge, castCharacterType, "Maria Elena");

        anEdge = g.NewEdge(castType, mManhattan, pDiane);
        g.SetProperty(anEdge, castCharacterType, "Mary");

        anEdge = g.NewEdge(castType, mManhattan, pWoody);
        g.SetProperty(anEdge, castCharacterType, "Isaac");

        // Add some DIRECTS edges
        anEdge = g.NewEdge(directsType, pSofia, mLostInTranslation);
        anEdge = g.NewEdge(directsType, pWoody, mVickyCB);
        anEdge = g.NewEdge(directsType, pWoody, mManhattan);

    // QUERIES
        // Get the movies directed by Woody Allen
        Elements directedByWoody = g.Neighbors(pWoody, directsType, EdgesDirection.Outgoing);

        // Get the cast of the movies directed by Woody Allen
        Elements castDirectedByWoody = g.Neighbors(directedByWoody, castType, EdgesDirection.Any);


        // Get the movies directed by Sofia Coppola
        Elements directedBySofia = g.Neighbors(pSofia, directsType, EdgesDirection.Outgoing);

        // Get the cast of the movies directed by Sofia Coppola
        Elements castDirectedBySofia = g.Neighbors(directedBySofia, castType, EdgesDirection.Any);

        // We want to know the people that acted in movies directed by Woody AND in movies directed by Sofia.
        Elements castFromBoth = Elements.CombineIntersection(castDirectedByWoody, castDirectedBySofia);

        // Say hello to the people found
        foreach (Element peopleOid in castFromBoth)
        {
          object value = g.GetProperty(peopleOid, peopleNameType);
          System.Console.WriteLine("Hello " + value);
        }
        graphId = session.Persist(g);
        session.Commit();
      }

      using (SessionNoServer session = new SessionNoServer(systemDir, false))
      {
        session.BeginRead();
        Graph g = (Graph) session.Open(graphId);
        TypeId movieType = g.FindType("MOVIE");
        PropertyId movieTitleProperty = g.FindProperty(movieType, "TITLE");
        Element obj = g.FindElement(movieTitleProperty, "Manhattan");
        session.Commit();
      }
    }
  }
}
