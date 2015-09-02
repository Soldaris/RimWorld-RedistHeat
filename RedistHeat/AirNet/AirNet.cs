﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RedistHeat
{
    public class AirNet
    {
        public readonly int debugId;
        private static int debugIdNext;

        public CompAir root;
        public readonly List< CompAir > nodes = new List< CompAir >();

        private float netTemperature;
        public float NetTemperature
        {
            get { return netTemperature; }
            set { netTemperature = Mathf.Clamp( value, -270, 2000 ); }
        }

        public NetLayer Layer { get; }
        public int LayerInt => (int) Layer;


        public AirNet( IEnumerable< CompAir > newNodes, NetLayer layer, CompAir root )
        {
            Layer = layer;
            foreach ( var current in newNodes )
            {
                RegisterNode( current );
                current.connectedNet = this;
            }

            checked
            {
                debugId = debugIdNext++;
            }
            this.root = root;

            var intake = newNodes.Where( s => s is CompAirTrader )?.Cast<CompAirTrader>().ToList().Find( s => Mathf.Approximately( s.props.energyPerSecond, 1 ) );
            if(intake == null || intake.netTemp == 999 )
            {
                NetTemperature = GenTemperature.OutdoorTemp;
            }
            else
            {
                NetTemperature = intake.netTemp;
            }
        }

        public void AirNetTick()
        {

        }

        public void RegisterNode( CompAir node )
        {
            if ( nodes.Contains( node ) )
            {
                Log.Error( "LT-RH: AirNet adding node " + node + " which it already has." );
            }
            else
            {
                nodes.Add( node );
            }
        }

        public void DeregisterNode( CompAir node )
        {
            nodes.Remove( node );
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append( "AirNet " )
                  .Append( debugId )
                  .Append( " (nodes count: " )
                  .Append( nodes.Count )
                  .Append( ", layer " )
                  .Append( Layer )
                  .Append( ", root " )
                  .Append( root.parent.Position )
                  .Append( ")" );
            return result.ToString();
        }
    }
}