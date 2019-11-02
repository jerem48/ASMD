﻿//___________________________________________________________________________________
//
//  Copyright (C) 2019, Mariusz Postol LODZ POLAND.
//
//___________________________________________________________________________________

using CAS.UA.Model.Designer.Properties;

namespace CAS.UA.Model.Designer.Wrappers
{
  internal class ParametersFolder: Folder
  {
    
    #region creators
    /// <summary>
    /// Initializes a new instance of the <see cref="ParametersFolder"/> class.
    /// </summary>
    /// <param name="parameters">The parameters to be added to the tree.</param>
    /// <param name="text">The node text.</param>
    /// <param name="toolStripTip">The tool strip tip text.</param>
    public ParametersFolder
      (
        Opc.Ua.ModelCompiler.Parameter[] parameters,
        string text,
        string toolStripTip
      )
      : base( text, toolStripTip )
    {
      TypesAvailableToBePasted.Add( typeof( Opc.Ua.ModelCompiler.Parameter ) );
      if ( parameters == null || parameters.Length == 0 )
        return;
      foreach ( Opc.Ua.ModelCompiler.Parameter rf in parameters )
        Add( new Parameter( rf ) );
    }
    #endregion

    #region public
    /// <summary>
    /// Gets the name of the help topic.
    /// </summary>
    /// <value>The name of the help topic.</value>
    public override string HelpTopicName
    {
      get { return Resources.NodeClasses_Objects_ParametersFolder; }
    }
    internal Opc.Ua.ModelCompiler.Parameter[] CreateParameters()
    {
      if ( Count == 0 )
        return null;
      int ii = 0;
      Opc.Ua.ModelCompiler.Parameter[] array = new Opc.Ua.ModelCompiler.Parameter[ Count ];
      foreach ( Parameter node in this )
        array[ ii++ ] = (Opc.Ua.ModelCompiler.Parameter)node.ModelDesignerNode;
      return array;
    }
    /// <summary>
    /// Gets the tree node and all children.
    /// </summary>
    /// <returns>
    /// The node of the type <see cref="System.Windows.Forms.TreeNode"/> with all children added to the Nodes collection.
    /// </returns>
    public override BaseDictionaryTreeNode GetTreeNode()
    {
      //TODO Refactoring application architecture - remove recursion #6
      throw new System.NotImplementedException("TrrNode factory must be implemented");
      //return new TreeNode( this );
    }
    public override NodeTypeEnum NodeType
    {
      get { return NodeTypeEnum.ParametersFolderNode; }
    }
    #endregion

    #region private
    // TODO Refactoring application architecture - remove recursion #6 - remove
    //private class ParametersFolderTreeNodeControl : FolderTreeNodeControl<ParametersFolder>
    //{
    //  public ParametersFolderTreeNodeControl( ParametersFolder parent )
    //    : base( parent )
    //  { }
    //  protected override void BeforeMenuStripOpening()
    //  {
    //    AddMenuItemPaste();
    //    AddMenuItemAdd(ModelEntity.ListOfNodes );
    //    base.BeforeMenuStripOpening();
    //  }
    //}
    internal INodeFactory[] ListOfNodes
    {
      get
      {
        TypeListItem<Parameter>[] m_list = new TypeListItem<Parameter>[1];
        m_list[0] = new TypeListItem<Parameter>();
        return m_list;
      }
    }
    #endregion

  }
}
