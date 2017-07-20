using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACS4Ingest
{
  public class Distributor
  {
    public Distributor() 
    {
      
    }

    const string builtin_distributor = "00000000-0000-0000-0000-000000000001";

    public enum DistributeDBColumnIndex
    {
      DIST_ID = 0,
      DIST_NAME,
      DIST_DESC,
      DIST_URL,
      DIST_NOTIFYURL,
      DIST_PUBLICKEY,
      DIST_SHAREDSECRET,
      DIST_MAXLOANCOUNT,
      DIST_LINKEXPIRATION
    };

    private string distributorName;
    public string DistributorName
    {
      get{
        return distributorName;
      }
      set{
        distributorName = value;
      }
    }

    private string distributorID;
    public string DistributorID
    {
      get
      {
        return distributorID;
      }
      set
      {
        distributorID = value;
      }
    }

    private byte[] sharedsecret;
    public byte[] SharedSecret
    {
      get
      {
        return sharedsecret;
      }
      set
      {
        sharedsecret = value;
      }
    }

    private string sharedsecret64; // base 64 encoded shared secret
    public string SharedSecret64
    {
      get
      {
        return sharedsecret64;
      }
      set
      {
        sharedsecret64 = value;
      }
    }

    private string distributorURL;
    public string DistributorURL
    {
      get
      {
        return distributorURL;
      }
      set
      {
        distributorURL = value;
      }
    }

    private string description;
    public string Description
    {
      get
      {
        return description;
      }
      set
      {
        description = value;
      }
    }

    private int maxLoanCount;
    public int MaxLoanCount
    {
      get
      {
        return maxLoanCount;
      }
      set
      {
        maxLoanCount = value;
      }
    }
    
    private int linkexpiration;
    public int LinkExpiration
    {
      get
      {
        return linkexpiration;
      }
      set
      {
        linkexpiration = value;
      }
    }
  }
}
