﻿//___________________________________________________________________________________
//
//  Copyright (C) 2019, Mariusz Postol LODZ POLAND.
//
//___________________________________________________________________________________

using CAS.UA.Model.Designer.ImportExport;
using CAS.UA.Model.Designer.Properties;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace CAS.UA.Model.Designer.Wrappers
{
  /// <summary>
  /// Root of the model tree.
  /// </summary>
  internal partial class ModelDesign : WrapperBase<Wrappers4ProperyGrid.ModelDesign, Opc.Ua.ModelCompiler.ModelDesign>
  {

    #region creators
    public ModelDesign()
      : this
      (
        new Opc.Ua.ModelCompiler.ModelDesign()
        {
          TargetNamespace = Settings.Default.TargetNamespace,
          Namespaces = new Opc.Ua.ModelCompiler.Namespace[]
            { new  Opc.Ua.ModelCompiler.Namespace()
               { Value = Settings.Default.TargetNamespace, 
                 XmlPrefix = Settings.Default.TargetNamespaceXmlPrefix, 
                 Name = Settings.Default.TargetNamespaceXmlPrefix 
               },
             new Opc.Ua.ModelCompiler.Namespace()
               { Value = OPCUATargetNamespace, 
                 XmlPrefix = Settings.Default.XmlUATypesPrefix, 
                 Name = Settings.Default.XmlUATypesPrefix 
               }
            }
        },
        false
      )
    { }
    public ModelDesign(Opc.Ua.ModelCompiler.ModelDesign node, bool library)
      : base(new Wrappers4ProperyGrid.ModelDesign(node))
    {
      TypesAvailableToBePasted.Add(typeof(Opc.Ua.ModelCompiler.NodeDesign));
      TypesAvailableToBePasted.Add(typeof(Opc.Ua.ModelCompiler.Namespace));
      m_Namespaces = new NamespacesFolder(node);
      Add(m_Namespaces);
      Opc.Ua.ModelCompiler.NodeDesign[] list = node.Items;
      if (list == null || list.Length == 0)
        return;
      foreach (var item in list)
        Add(NodeFactory.Create(item));
      if (library)
        OPCUATargetNamespace = Wrapper.TargetNamespace;
    }
    #endregion

    #region private
    internal INodeFactory[] ListOfNodes
    {
      get
      {
        List<INodeFactory> m_list = new List<INodeFactory>();
        m_list.Add(new TypeListItem<DataTypeDesign>());
        m_list.Add(new TypeListItem<DictionaryDesign>());
        m_list.Add(new TypeListItem<MethodDesign>());
        m_list.Add(new TypeListItem<ObjectDesign>());
        m_list.Add(new TypeListItem<ObjectTypeDesign>());
        m_list.Add(new TypeListItem<ReferenceTypeDesign>());
        m_list.Add(new TypeListItem<VariableTypeDesign>());
        m_list.Add(new TypeListItem<VariableDesign>());
        m_list.Add(new TypeListItem<PropertyDesign>());
        m_list.Add(new TypeListItem<ViewDesign>());
        return m_list.ToArray();
      }
    }
    internal void GetImportMenu(System.Windows.Forms.ToolStripItemCollection items)
    {
      if (this.TestIfReadOnlyAndRetrunTrueIfReadOnly())
        return;
      OPCDA.CreateImportMenuItems(items, new EventHandler(CreateImportMenuClick));
    }
    protected override string NodeName()
    {
      return "Domain";
    }
    protected override string NodeTip()
    {
      string ns;
      if (Wrapper == null || Wrapper.TargetNamespace == null)
        ns = "< empty > \n";
      else
        ns = Wrapper.TargetNamespace;
      return String.Format(WrapperResources.ModelDesignNodeToolTipText, ns);
    }
    private NamespacesFolder m_Namespaces;
    private void CreateImportMenuClick(object sender, EventArgs e)
    {
      Opc.Ua.ModelCompiler.NodeDesign[] nodes = OPCDA.OnImportMenuItemClick(GetTargetNamespace());
      if (nodes == null)
        return;
      List<BaseTreeNode> arr = new List<BaseTreeNode>();
      bool CancelWasPressed = false;
      foreach (var item in nodes)
      {
        BaseTreeNode newNode = NodeFactory.Create(item);
        arr.Add(newNode);
        if (item is Opc.Ua.ModelCompiler.InstanceDesign)
          this.CreateInstanceConfigurations(newNode, CancelWasPressed, out CancelWasPressed);
      }
      this.AddRange(arr.ToArray());
    }
    #endregion

    #region public
    /// <summary>
    /// Gets the name of the help topic.
    /// </summary>
    /// <value>The name of the help topic.</value>
    public override string HelpTopicName
    {
      get { return Resources.NodeClasses_Model; }
    }
    /// <summary>
    /// Gets the node class.
    /// </summary>
    /// <value>The node class.</value>
    public override NodeClassesEnum NodeClass
    {
      get { return NodeClassesEnum.None; }
    }
    public override NodeTypeEnum NodeType
    {
      get { return NodeTypeEnum.ModelNode; }
    }
    public override Dictionary<FolderType, IEnumerable<IModelNodeAdvance>> GetFolders()
    {
      Dictionary<FolderType, IEnumerable<IModelNodeAdvance>> tobereturned = base.GetFolders();
      tobereturned.Add(FolderType.ModelNodes, this);
      return tobereturned;
    }
    public override void MenuItemCut_Action()
    {
      MenuItemCopy_Action();
      this.MessageBoxHandling.Show(Resources.WrapperTreeNode_menu_cut_cannot_be_cut);
    }
    public override void MenuItemPaste_Action()
    {
      object DeserializedNode = GetModelDesignerNodeFromStringRepresentationFromClipboard();
      BaseTreeNode baseTreeNode = NodeFactory.Create(DeserializedNode);
      if (DeserializedNode is Opc.Ua.ModelCompiler.NodeDesign)
      {
        this.Add(baseTreeNode);
        return;
      }
      else if (DeserializedNode is Opc.Ua.ModelCompiler.Namespace)
      {
        this.m_Namespaces.Add(baseTreeNode);
        return;
      }
      this.MessageBoxHandling.Show(Resources.WrapperTreeNode_menu_paste_cannot_be_done);
    }
    public override string[] AvailiableNamespaces
    {
      get
      {
        return m_Namespaces.GetAvailableNamespaces();
      }
    }
    /// <summary>
    /// Gets the model designer node <see cref="Opc.Ua.ModelCompiler.ModelDesign"/>.
    /// </summary>
    /// <value>The model designer node <see cref="Opc.Ua.ModelCompiler.ModelDesign"/>.</value>
    public override object ModelDesignerNode
    {
      get
      {
        Opc.Ua.ModelCompiler.ModelDesign node = (Opc.Ua.ModelCompiler.ModelDesign)base.ModelDesignerNode;
        node.Namespaces = m_Namespaces.Namespaces;
        List<Opc.Ua.ModelCompiler.NodeDesign> uaNodes = new List<Opc.Ua.ModelCompiler.NodeDesign>();
        foreach (BaseTreeNode item in this)
        {
          IParent tn = item as IParent;
          if (tn == null)
            continue;
          uaNodes.Add((Opc.Ua.ModelCompiler.NodeDesign)tn.ModelDesignerNode);
        }
        node.Items = uaNodes.ToArray();
        return node;
      }
    }
    #endregion

    #region internal
    internal bool SaveModel(string filePath)
    {
      XmlFile.DataToSerialize<Opc.Ua.ModelCompiler.ModelDesign> _config;
      _config.Data = ModelDesignerNode as Opc.Ua.ModelCompiler.ModelDesign;
      _config.XmlNamespaces = XmlNamespaces;
      _config.StylesheetName = "OPCFModelDesign.xslt";
      XmlFile.WriteXmlFile<Opc.Ua.ModelCompiler.ModelDesign>( _config, filePath, System.IO.FileMode.Create );
      return true;
    }
    /// <summary>
    /// Gets the XML namespaces to be used for serialization process.
    /// </summary>
    /// <value>The XML namespaces <see cref="XmlSerializerNamespaces"/>.</value>
    internal XmlSerializerNamespaces XmlNamespaces { get { return m_Namespaces.XmlNamespaces; } }
    internal static string OPCUATargetNamespace { get; private set; }
    /// <summary>
    /// Gets the target namespace of the current project.
    /// </summary>
    /// <returns>The target namespace.</returns>
    public override string GetTargetNamespace()
    {
      return Wrapper.TargetNamespace;
    }
    /// <summary>
    /// All items in this collection of <see cref="INodeDesign"/> is added to the <paramref name="space"/>
    /// </summary>
    /// <param name="space">The address space.</param>
    internal void AddNode2AddressSpace(IAddressSpaceCreator space)
    {
      foreach (var item in this)
      {
        INodeDesign modelItem = item as INodeDesign;
        if (modelItem != null)
          modelItem.AddNode2AddressSpace(space);
      }
    }
    internal ITypeDesign FindType(XmlQualifiedName myType)
    {
      ITypeDesign ret = null;
      foreach (var item in this)
      {
        INodeDesign node = item as INodeDesign;
        if ((node != null) && (node.SymbolicName == myType))
        {
          ret = node as ITypeDesign;
          if (ret != null)
            break;
        }
      }
      return ret;
    }
    #endregion

  }
}
