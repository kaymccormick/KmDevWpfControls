using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KmDevWpfControls;

namespace ControlsDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var deserialize = JsonSerializer.Deserialize<List<XX>>(File.ReadAllText(@"C:\temp\editors1.json"));
            GrovelTypes();
            foreach (var (item1, item2) in deserialize.Select(xx => Tuple.Create(Type.GetType(xx.Item1), Type.GetType(xx.Item2))))
            {
                EditorTypes.Add(new TypeNode2(new NamespaceType() { Type = item1 }));
            }


            InitializeComponent();
            AllowDrop = true;
            tt.RootItems = EditorTypes;
            VisualTreeViewModel.RootVisual = this;
        }

        public static void GrovelTypes()
        {
            LoadAssemblies();

            //EditorSet = EditorTypes.ToHashSet();
         
            var selectMany = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a =>
            {
                try
                {
                    return a.GetExportedTypes();
                }
                catch (Exception)
                {
                    return Enumerable.Empty<Type>();
                }
            });
            Dictionary<string, object> enums = new Dictionary<string, object>();
            List<Tuple<string, string>> editors = new List<Tuple<string, string>>();
            foreach (var type in selectMany)
            {
                if (type.IsEnum)
                {
                    enums[type.FullName] = new
                    {
                        Flags = type.GetCustomAttributes<FlagsAttribute>().Any(),
                        Count = Enum.GetValues(type).Length,
                        D = Enum.GetNames(type).ToDictionary(s => s, s => Enum.Parse(type, s))
                    };
                }

            

                var p = TypeDescriptor.GetEditor(type, typeof(UITypeEditor));
                if (p != null)
                {
                    editors.Add(Tuple.Create(type.AssemblyQualifiedName, p.GetType().FullName));
                }
            }

            File.WriteAllText(@"C:\temp\enums.json",
                JsonSerializer.Serialize(enums, new JsonSerializerOptions() { WriteIndented = true }));

            File.WriteAllText(@"C:\temp\editors.json",
                JsonSerializer.Serialize(editors, new JsonSerializerOptions() {WriteIndented = true}));
            foreach (var type in new[] {typeof(EditorAttribute), typeof(TypeConverterAttribute)})
            {
                var out1 = new List<IEnumerable<Tuple<List<string>, IEnumerable<Tuple<string, string>>>>>();
//                List<Tuple<string, string, string>> c = new List<Tuple<string, string, string>>();
                foreach (var type1 in selectMany.Where(zz => zz.GetCustomAttributes(type).Any()))
                {
                    IEnumerable<Tuple<List<string>, IEnumerable<Tuple<string, string>>>> enumerable = type1.CustomAttributes
                        .Where(z => z.AttributeType == type).Select(zz =>
                            Tuple.Create(zz.ConstructorArguments.Select(zzz => zzz.Value.ToString()).ToList(),
                                zz.NamedArguments.Select(Z => Tuple.Create(Z.MemberName, Z.TypedValue.Value.ToString()))));
                    out1.Add(enumerable);

                    //c.Add(Tuple.Create(type.FullName, type1.FullName, converterTypeName));
                }

                File.WriteAllText(@"C:\temp\" + type.Name + ".json",
                    JsonSerializer.Serialize(out1, new JsonSerializerOptions() {WriteIndented = true}));
            }

            foreach (var grouping in selectMany
                .Select(x => new {x, x.CustomAttributes}).Where(z => z.CustomAttributes.Any()).SelectMany(z =>
                    z.CustomAttributes.Select(zz => new {zz, z.x}).GroupBy(arg => arg.zz.AttributeType.FullName)))
            {
                switch (grouping.Key)
                {
                    //System.Reflection.DefaultMemberAttribute
                    //System.ComponentModel.TypeConverterAttribut
                    case "System.Runtime.CompilerServices.TypeForwardedFromAttribute":
                        continue;
                }

                foreach (var x1 in grouping)
                    Debug.WriteLine(
                        $"{grouping.Key} {x1.x.FullName} {String.Join(", ", x1.zz.ConstructorArguments)} {String.Join(", ", x1.zz.NamedArguments.Select(argument => $"{argument.MemberName}={argument.TypedValue.Value}"))}");
            }
        }

        private static void LoadAssemblies()
        {
            var loaded = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.FullName).ToHashSet();

            void RecursiveLoad(AssemblyName name)
            {
                if (loaded.Contains(name.FullName))
                    return;
                loaded.Add(name.FullName);
                Assembly a;
                try
                {
                    a = Assembly.Load(name);
                }
                catch
                {
                    return;
                }

                foreach (var assemblyName in a.GetReferencedAssemblies().Where(x => !loaded.Contains(x.FullName)))
                    RecursiveLoad(assemblyName);
            }

            foreach (var name in Assembly.GetExecutingAssembly().GetReferencedAssemblies()) RecursiveLoad(name);
        }

        public HashSet<Type> EditorSet { get; set; }

        public ObservableCollection<TypeNode2> EditorTypes { get; set; } = new ObservableCollection<TypeNode2>();

        public VisualTreeViewModel VisualTreeViewModel { get; set; } = new VisualTreeViewModel();

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            if (!e.Handled)
            {
                e.Effects = e.AllowedEffects;
                e.Handled = true;
            }
        }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            foreach (var format in e.Data.GetFormats())
            {
                Debug.WriteLine($"{format}");
                try
                {
                    Debug.WriteLine($"{e.Data.GetData(format)}");
                }
                catch
                {
                }
            }
        }
    }

    public class XX
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
    }
}