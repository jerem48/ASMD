﻿//___________________________________________________________________________________
//
//  Copyright (C) 2019, Mariusz Postol LODZ POLAND.
//
//___________________________________________________________________________________


using CAS.UA.IServerConfiguration;
using CAS.UA.Model.Designer.Properties;
using System;
using System.Collections.Generic;
using BaseModelType = Opc.Ua.ModelCompiler.VariableDesign;

namespace CAS.UA.Model.Designer.Wrappers
{
  /// <summary>
  /// <see cref="TreeNode"/> representing <see cref="Opc.Ua.ModelCompiler.VariableDesign"/> in the model structure
  /// </summary>
  internal partial class VariableDesign: VariableDesignGeneric<Wrappers4ProperyGrid.VariableDesign<BaseModelType>, BaseModelType>
  {
    #region creators
    public VariableDesign()
      : base( new Wrappers4ProperyGrid.VariableDesign<BaseModelType>( new BaseModelType() ) )
    { }
    public VariableDesign( BaseModelType node )
      : base( new Wrappers4ProperyGrid.VariableDesign<BaseModelType>( node ), node )
    { }
    #endregion

    #region private
    private class TreeNode: VariableDesignGeneric<Wrappers4ProperyGrid.VariableDesign<BaseModelType>, BaseModelType>.TreeNode<VariableDesign>
    {
      public TreeNode( VariableDesign parent )
        : base( parent )
      { }
      protected override void BeforeMenuStripOpening()
      {
        AddMenuItemGoTo( Resources.WrapperTreeNodeAddMenuItemGoto
          + Resources.WrapperTreeNodeAddMenuItemGoto_DataType,
          Resources.WrapperTreeNodeAddMenuItemGoto_DataType_tooltip,
          new EventHandler( AddMenuItemGoTo_Click ) );
        base.BeforeMenuStripOpening();
      }
      private void AddMenuItemGoTo_Click( object sender, System.EventArgs e )
      {
        TreeView.GoToNode(ModelEntity.Wrapper.DataType.XmlQualifiedName );
      }
      internal override Dictionary<string, System.Xml.XmlQualifiedName> GetCoupledNodesXmlQualifiedNames()
      {
        var list = base.GetCoupledNodesXmlQualifiedNames();
        if (ModelEntity.Wrapper.DataType.XmlQualifiedName != null && !ModelEntity.Wrapper.DataType.XmlQualifiedName.IsEmpty )
          list.Add( Resources.WrapperTreeNodeAddMenuItemGoto_DataType,
            ModelEntity.Wrapper.DataType.XmlQualifiedName );
        return list;
      }
    }
    protected override void AddNodeDescriptors( List<INodeDescriptor> dsptrs, UniqueIdentifier ui )
    {
      ui.Update( false, Wrapper.SymbolicName.XmlQualifiedName, false );
      dsptrs.Add( Wrapper.GetINodeDescriptor( ui, this.NodeClass ) );
      base.AddNodeDescriptors( dsptrs, ui );
    }
    #endregion

    #region public
    /// <summary>
    /// Gets the name of the help topic.
    /// </summary>
    /// <value>The name of the help topic.</value>
    public override string HelpTopicName
    {
      get { return Resources.NodeClasses_Objects_Variable; }
    }
    /// <summary>
    /// Gets the node class.
    /// </summary>
    /// <value>The node class.</value>
    public override NodeClassesEnum NodeClass
    {
      get { return NodeClassesEnum.Variable; }
    }
    /// <summary>
    /// Gets the tree node and all children.
    /// </summary>
    /// <returns>
    /// The node of the type <see cref="System.Windows.Forms.TreeNode"/> with all children added to the Nodes collection.
    /// </returns>
    public override BaseDictionaryTreeNode GetTreeNode()
    {
      return new TreeNode( this );
    }
    #endregion
  }
  internal abstract partial class VariableDesignGeneric<type, OPCType>: InstanceDesign<type, OPCType>
    where type: Wrappers4ProperyGrid.VariableDesign<OPCType>
    where OPCType: BaseModelType, new()
  {

    #region creators
    public VariableDesignGeneric( type child )
      : base( child )
    {
    }
    public VariableDesignGeneric( type child, OPCType node )
      : base( child, node )
    {
    }
    #endregion

  }
}
