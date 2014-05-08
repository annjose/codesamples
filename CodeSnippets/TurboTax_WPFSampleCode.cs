Add an image owned by Engine team (reference: D:\Dev\Main\WNTA\Service\EasyStep\EasyStep\ElectronicFiling\ElectronicFilingSuccessHoldUC.xaml)
                            <Image
                            Width="55px"
                            Height="18px"
                            x:Name="TwitterShare" 
                            Cursor="Hand"
                            Margin="0px 0 15 0px"
                            Source="{Binding Converter={StaticResource findImageConverter}, ConverterParameter=twitter_share.png}"
                            RenderOptions.BitmapScalingMode="HighQuality"
                            RenderOptions.EdgeMode="Aliased"/>

Add an image owned by UX team (reference: D:\Dev\Main\WNTA\Service\EasyStep\EasyStep\Import\ImportSummaryUC.xaml)

                <Image
                    Grid.Column="0"
                    Grid.Row="1"
                    Grid.ColumnSpan="1"
                    Margin="0,0,0,0"
										Source="{Binding Converter={StaticResource findImageConverter}, ConverterParameter=facebook_share.png}"
                    interviewVm:InterviewImageBehavior.ImageSource="intgfx_easy_import_horizontal.png" />