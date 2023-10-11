using System.Diagnostics;
using System.Globalization;
using System.Windows.Markup;

namespace HighlightWpfApp
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ExampleText : ObservableObject
    {
        public const string DefaultCultureName = "en-US";

        public ExampleText()
            : this(string.Empty, string.Empty, DefaultCultureName)
        { }

        public ExampleText(string title, string text, CultureInfo culture)
            : this(title, text, culture.Name)
        { }

        public ExampleText(string title, string text, string cultureName)
        {
            _Title = title;
            _Text = text;
            _CultureName = cultureName;
            _XmlLanguage = XmlLanguage.GetLanguage(cultureName);
        }

        private string _Title = string.Empty;

        public string Title
        {
            get => _Title;
            private set => Set(ref _Title, value);
        }

        private string _Text = string.Empty;

        public string Text
        {
            get => _Text;
            private set => Set(ref _Text, value);
        }

        private string _CultureName;

        public string CultureName
        {
            get => _CultureName;
            private set
            {
                if (Set(ref _CultureName, value))
                {
                    XmlLanguage = XmlLanguage.GetLanguage(value);
                }
            }
        }

        private XmlLanguage _XmlLanguage;

        public XmlLanguage XmlLanguage
        {
            get => _XmlLanguage;
            private set => Set(ref _XmlLanguage, value);
        }

        internal string DebuggerDisplay
        {
            get => $"({CultureName})->{Text}";
        }
    }

    public static class ExampleTexts
    {
        public static readonly ExampleText[] Data = new ExampleText[]
        {
            new ExampleText(
                title: "Gettysburg Address",
                text: @"Four score and seven years ago our fathers brought forth, upon this continent, a new nation, conceived in Liberty, and dedicated to the proposition that all men are created equal.

Now we are engaged in a great civil war, testing whether that nation, or any nation so conceived, and so dedicated, can long endure. We are met on a great battle-field of that war. We have come to dedicate a portion of that field, as a final resting place for those who here gave their lives, that that nation might live. It is altogether fitting and proper that we should do this.

But, in a larger sense, we can not dedicate, we can not consecrate -- we can not hallow -- this ground. The brave men, living and dead, who struggled here, have consecrated it far above our poor power to add or detract. The world will little note, nor long remember what we say here, but it can never forget what they did here. It is for us the living, rather, to be dedicated here to the unfinished work which they who fought here, have, thus far, so nobly advanced. It is rather for us to be here dedicated to the great task remaining before us -- that from these honored dead we take increased devotion to that cause for which they gave the last full measure of devotion -- that we here highly resolve that these dead shall not have died in vain -- that this nation, under God, shall have a new birth of freedom -- and that government of the people, by the people, for the people, shall not perish from the earth.",
                cultureName: "en-US"
            ),

            new ExampleText(
                title: "Shakespeare Hamlet soliloquy",
                text: @"“To be, or not to be: that is the question:
Whether ’tis nobler in the mind to suffer
The slings and arrows of outrageous fortune,
Or to take arms against a sea of troubles,
And by opposing end them. To die: to sleep;”",
                cultureName: "en-US"
            ),

            new ExampleText(
                title: "The Gold Star (Estrellita de Oro)",
                text: @"Había una vez un rey viudo que tenía una hija muy bella. Unos años después de haber perdido a su esposa conoció a una mujer que también tenía una hija y decidió volver a casarse de nuevo.

Tras su matrimonio, la mujer se volvió cruel con la hija del rey. Le tenía mucha envidia porque era mucho más bella que su hija y como castigo la mandaba todos los días a lavar al río. La joven se resignaba y cumplía con las órdenes de su madrastra pues sabía que a pesar de todo, su padre era feliz junto a ella.

Un día, estando la muchacha en el río una mujer que lavaba a su lado perdió un anillo.

- ¡Se me ha caído! ¡Se me ha caído mi anillo! ¡Ayúdame a recuperarlo hija mía, que tengo muy mala vista y no lo veo por ninguna parte!
- No se preocupe señora - dijo la muchacha sumergiendo su brazo en el agua sucia y helada.

Pero el anillo no aparecía y tanto tuvo que agacharse la muchacha para tratar de recuperarlo que acabó dándose con algo en la frente.

Afortunadamente el golpe mereció la pena ya que gracias a él pudo recuperar el anillo. Aunque sucedió algo extraño… en el lugar donde la joven se había golpeado, la frente, comenzó a salirle una estrella.

Al llegar a casa la madrastra de la muchacha le dijo en cuanto la vio:

- ¿Qué es eso que llevas ahí?

La muchacha le contó avergonzada lo ocurrido y dijo no entender cómo le había salido aquella estrella.

- ¡Mañana serás tú quien vaya a lavar! - le dijo a su otra hija
- ¿Yo? ¡Ni hablar!¡Que vaya ella!
- Ella ya tiene su estrella así que irás tú y punto ¿O es que vas a dejar que tu hermana tenga algo que tu no?

La muchacha fue al río y como no había forma de que se decidiera a tocar el agua porque estaba muy sucia, la madre acabó metiéndole la cabeza a la fuerza en el río. Pero lo peor no fue que la metiera, sino que la sacara…

- ¿¿Pero qué es eso?? - dijo la madre atónita mientras señalaba asustada frente de su hija

La hija, que se imaginó algo terrible en cuanto vio la cara de susto de la madre, se llevó las manos a la frente y gritó con todas sus fuerzas al darse cuenta de que lo que tenía en la frente no era una estrella sino un rabo de burro.

- ¡Rápido, al médico! ¡Tenemos que quitarte eso de la frente!- dijo la madre mientras la hija lloraba.

El médico decidió que lo mejor era cortárselo a ras y para disimularlo le pusieron un velo.

Al llegar a casa se encontraron con la carroza real en la misma puerta de su casa. En ese momento la madre recordó que hacía un tiempo que el príncipe se encontraba llamando de puerta en puerta para elegir a su esposa de entre todas las mujeres del reino. Y precisamente había llegado a su casa ese mismo día.

-Estrellita de Oro Ayúdame a encerrar a tu hermana en el desván. No podemos dejar que el p?incipe vea que es ella quien tiene la estrella de oro.
- ¡Sí madre!

La madre le dijo al príncipe que era su hija, la que portaba el velo, quien había tenido la gracia de recibir la estrella de oro así que el príncipe la subió en su carroza creyendo que era ella la muchacha a quien estaba buscando para convertirla en su esposa.

Pero de repente, los caballos de la carroza dieron un traspiés y a la muchacha se le cayó el velo de la frente de la sacudida.

- ¿Qué es eso? ¡Eso no es una estrella, es una cola de burro! - dijo el príncipe enfadado cuando se dio cuenta de que habían tratado de engañarlo.

Rápidamente volvió a la casa de la muchacha y allí encontró encerrada en el desván a la muchacha que en realidad buscaba, su estrella de oro.

Volvieron a palacio, se casaron, fueron felices y comieron perdices.",
                cultureName: "es-ES"
            ),

            new ExampleText(
                title: "Italian sayings",
                text: @"From https://www.fluentu.com/blog/italian/italian-sayings-quotes-proverbs/

1. Vino rosso fa buon sangue – “Red wine makes good blood”
2. Non puoi avere sia la botte piena, che la moglie ubriaca – “You can’t have a full wine barrel and a drunk wife”
3. Anni e bicchieri di vino non si contano mai – “You never count years or glasses of wine”
4. L’acqua fa male e il vino fa cantare – “Water is bad and wine makes you sing”
5. Chi non beve in compagnia o è un ladro o è una spia – “If you don’t drink in company, you’re either a thief or a spy”",
                cultureName: "it-IT"
            ),

            // From https://ar.wikisource.org/wiki/%D8%A3%D9%84%D9%81_%D9%84%D9%8A%D9%84%D8%A9_%D9%88%D9%84%D9%8A%D9%84%D8%A9/%D8%A7%D9%84%D8%AC%D8%B2%D8%A1_%D8%A7%D9%84%D8%A3%D9%88%D9%84
            new ExampleText(
                title: "Arabic Exerpt The Thousand and One Nights",
                text: @"ففي الليلة الأولى قالت: بلغني أيها الملك السعيد أنه كان تاجر من التجار كثير المال والمعاملات في البلاد قد ركب يومًا وخرج يطالب في بعض البلاد فاشتد عليه الحر فجلس تحت شجرة وحط يده في خرجه وأكل كسرة كانت معه وتمرة فلما فرغ من أكل التمرة رمى النواة وإذا هو بعفريت طويل القامة وبيده سيف فدنا من ذلك التاجر وقال له: قم حتى أقتلك مثل ما قتلت ولدي فقال له التاجر: كيف قتلت ولدك قال له: لما أكلت التمرة ورميت نواتها جاءت النواة في صدر ولدي فقضي عليه ومات من ساعته فقال التاجر للعفريت: أعلم أيها العفريت أني على دين ولي مال كثير وأولاد وزوجة وعندي",
                cultureName: "ar-AR"
            ),
        };
    }
}
