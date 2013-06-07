﻿// This sample is a preview of VelocityGraph, help us define the best possible graph api! The start of this api is inspired by Dex, see http://www.sparsity-technologies.com/dex.php,
// the sample here mimics what the dex sample is doing. Dex is considered by some to have the best performing graph api currently available. Unlike Dex, which is a C++/C implemented api,
// VelocityGraph is all implemented in C# and enable any type of Element values. VelocityGraph is provided as open source on GitHub, https://github.com/VelocityDB/VelocityGraph. 
// Anyone is welcome to contribute! VelocityGraph is built on top of VelocityDB.
// You will need a VelocityDB license to use VelocityGraph. Eventually VelocityGraph may have its own web site, http://www.VelocityGraph.com/

using System;
using System.IO;
using System.Linq;
using VelocityDb.Session;
using VelocityGraph;
using PropertyTypeId = System.Int32;
using PropertyId = System.Int32;
using VertexTypeId = System.Int32;
using EdgeTypeId = System.Int32;

namespace VelocityGraphSample
{
  using Vertexes = System.Collections.Generic.HashSet<Vertex>;
  using Edges = System.Collections.Generic.HashSet<Edge>;
  using System.Collections.Generic;
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
        VertexTypeId movieType = g.NewVertexType("MOVIE");
        PropertyTypeId movieIdType = g.NewVertexProperty(movieType, "ID", DataType.Long, PropertyKind.Unique);
        PropertyId movieTitleType = g.NewVertexProperty(movieType, "TITLE", DataType.String, PropertyKind.Indexed);
        PropertyId movieYearType = g.NewVertexProperty(movieType, "YEAR", DataType.Integer, PropertyKind.Indexed);

        // Add a node type for the people, with a unique identifier and an indexed Property
        VertexTypeId peopleType = g.NewVertexType("PEOPLE");
        PropertyId peopleIdType = g.NewVertexProperty(peopleType, "ID", DataType.Long, PropertyKind.Unique);
        PropertyId peopleNameType = g.NewVertexProperty(peopleType, "NAME", DataType.String, PropertyKind.Indexed);

        // Add an undirected edge type with an Property for the cast of a movie
        EdgeTypeId castType = g.NewEdgeType("CAST", false, false);
        PropertyId castCharacterType = g.NewEdgeProperty(castType, "CHARACTER", DataType.String, PropertyKind.Basic);

        // Add a directed edge type restricted to go from people to movie for the director of a movie
        EdgeTypeId directsType = g.NewRestrictedEdgeType("DIRECTS", peopleType, movieType, false);

    // DATA
        // Add some MOVIE nodes

        Vertex mLostInTranslation = g.NewVertex(movieType);
        mLostInTranslation.SetProperty(movieIdType, (long)1);
        mLostInTranslation.SetProperty(movieTitleType, "Lost in Translation");
        mLostInTranslation.SetProperty(movieYearType, (int) 2003);

        Vertex mVickyCB = g.NewVertex(movieType);
        mVickyCB.SetProperty(movieIdType, (long) 2);
        mVickyCB.SetProperty(movieTitleType, "Vicky Cristina Barcelona");
        mVickyCB.SetProperty(movieYearType, (int)2008);

        Vertex mManhattan = g.NewVertex(movieType);
        mManhattan.SetProperty(movieIdType, (long) 3);
        mManhattan.SetProperty(movieTitleType, "Manhattan");
        mManhattan.SetProperty(movieYearType, (int) 1979);


        // Add some PEOPLE nodes
        Vertex pScarlett = g.NewVertex(peopleType);
        pScarlett.SetProperty(peopleIdType, (long)1);
        pScarlett.SetProperty(peopleNameType, "Scarlett Johansson");

        Vertex pBill = g.NewVertex(peopleType);
        pBill.SetProperty(peopleIdType, (long) 2);
        pBill.SetProperty(peopleNameType, "Bill Murray");

        Vertex pSofia = g.NewVertex(peopleType);
        pSofia.SetProperty(peopleIdType, (long)3);
        pSofia.SetProperty(peopleNameType, "Sofia Coppola");

        Vertex pWoody = g.NewVertex(peopleType);
        pWoody.SetProperty(peopleIdType, (long)4);
        pWoody.SetProperty(peopleNameType, "Woody Allen");

        Vertex pPenelope = g.NewVertex(peopleType);
        pPenelope.SetProperty(peopleIdType, (long) 5);
        pPenelope.SetProperty(peopleNameType, "Penélope Cruz");

        Vertex pDiane = g.NewVertex(peopleType);
        pDiane.SetProperty(peopleIdType, (long)6);
        pDiane.SetProperty(peopleNameType, "Diane Keaton");

        // Add some CAST edges
        Edge anEdge;
        anEdge = g.NewEdge(castType, mLostInTranslation, pScarlett);
        anEdge.SetProperty(castCharacterType, "Charlotte");

        anEdge = g.NewEdge(castType, mLostInTranslation, pBill);
        anEdge.SetProperty(castCharacterType, "Bob Harris");

        anEdge = g.NewEdge(castType, mVickyCB, pScarlett);
        anEdge.SetProperty(castCharacterType, "Cristina");

        anEdge = g.NewEdge(castType, mVickyCB, pPenelope);
        anEdge.SetProperty(castCharacterType, "Maria Elena");

        anEdge = g.NewEdge(castType, mManhattan, pDiane);
        anEdge.SetProperty(castCharacterType, "Mary");

        anEdge = g.NewEdge(castType, mManhattan, pWoody);
        anEdge.SetProperty(castCharacterType, "Isaac");

        // Add some DIRECTS edges
        anEdge = g.NewEdge(directsType, pSofia, mLostInTranslation);
        anEdge = g.NewEdge(directsType, pWoody, mVickyCB);
        anEdge = g.NewEdge(directsType, pWoody, mManhattan);

    // QUERIES
        // Get the movies directed by Woody Allen
        Vertexes directedByWoody = pWoody.Neighbors(directsType, EdgesDirection.Outgoing);

        // Get the cast of the movies directed by Woody Allen
        Vertexes castDirectedByWoody = g.Neighbors(directedByWoody, castType, EdgesDirection.Any);

        // Get the movies directed by Sofia Coppola
        Vertexes directedBySofia = pSofia.Neighbors(directsType, EdgesDirection.Outgoing);

        // Get the cast of the movies directed by Sofia Coppola
        Vertexes castDirectedBySofia = g.Neighbors(directedBySofia, castType, EdgesDirection.Any);

        // We want to know the people that acted in movies directed by Woody AND in movies directed by Sofia.
        IEnumerable<Vertex> castFromBoth = castDirectedByWoody.Intersect(castDirectedBySofia);

        // Say hello to the people found
        foreach (Vertex person in castFromBoth)
        {
          object value = person.GetProperty(peopleNameType);
          System.Console.WriteLine("Hello " + value);
        }
        graphId = session.Persist(g);
        session.Commit();
      }

      using (SessionNoServer session = new SessionNoServer(systemDir, false))
      {
        session.BeginRead();
        Graph g = (Graph) session.Open(graphId);
        VertexTypeId movieType = g.FindVertexType("MOVIE");
        PropertyId movieTitleProperty = g.FindVertexProperty(movieType, "TITLE");
        Vertex obj = g.FindElement(movieTitleProperty, "Manhattan");
        session.Commit();
      }
    }
  }
}
